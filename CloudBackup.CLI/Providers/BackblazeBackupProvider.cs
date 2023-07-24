using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using CloudBackup.CLI.Abstract;
using CloudBackup.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CloudBackup.CLI.Providers;

public class BackblazeBackupProvider : BackupProvider<BackblazeSettings>
{
    public BackblazeBackupProvider(string baseFolder, BackblazeSettings settings, ILogger logger) : base(baseFolder, settings, logger)
    {
    }

    public override string Type => "Backblaze";

	protected override async Task<IDisposable> GetClientAsync()
	{
		var client = new BackblazeClient();
		await client.ConnectAsync(Settings.PublicKey, Settings.SecretKey);
		return client;
	}

	protected override async Task<IEnumerable<Abstract.File>> GetCloudFilesAsync(IDisposable? client, string folder)
	{
		var backblazeClient = client as BackblazeClient ?? throw new Exception("Unexpected client object");

		var bucket = await backblazeClient.Buckets.FindByNameAsync(Settings.BucketName);

		if (bucket is null)
		{
			var bucketResponse = await backblazeClient.Buckets.CreateAsync(Settings.BucketName, BucketType.AllPrivate);
			bucketResponse.EnsureSuccessStatusCode();
			bucket = bucketResponse.Response;
		}

		List<Abstract.File> results = new();
		do
		{
			var response = await backblazeClient.Files.ListNamesAsync(new ListFileNamesRequest(bucket.BucketId)
			{
				Prefix = folder,
				MaxFileCount = 200,
				// Backblaze has a weird way (IMO) of getting the next "page" of results. You give it a file name,
				// and it returns that (if found) with a max count of results following that file, sorted alphabetically I guess.
				// To keep from returning a file more than once, I append an underscore to the last file in the result set
				// so that it will skip that and move to the next. Presumably, the underscore makes it a non-existent file.
				// I'm trying to prevent returning a page's last file as the first file of the next page.				
				StartFileName = results.Any() ? results.Last().Path + "_" : default
			});
			response.EnsureSuccessStatusCode();
			var page = response.Response.Files.Select(fi => new Abstract.File(fi.FileName, fi.ContentLength, fi.UploadTimestamp));
			if (!page.Any()) break;
			results.AddRange(page);
		} while (true);

		return results;
	}

	protected override async Task UploadFileAsync(IDisposable? client, string basePath, Abstract.File file)
	{
		var backblazeClient = client as BackblazeClient ?? throw new Exception("Unexpected client object");

		var bucket = await backblazeClient.Buckets.FindByNameAsync(Settings.BucketName);
		var path = Path.Combine(basePath, file.Path);
		using var stream = System.IO.File.OpenRead(path);
		var response = await backblazeClient.UploadAsync(bucket.BucketId, file.Path, stream);
		response.EnsureSuccessStatusCode();
    }
}
