﻿using Bytewizer.Backblaze.Client;
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
			var response = await backblazeClient.Buckets.CreateAsync(Settings.BucketName, BucketType.AllPrivate);
			response.EnsureSuccessStatusCode();
			bucket = response.Response;
		}

		var results = await backblazeClient.Files.ListNamesAsync(bucket.BucketId);
		results.EnsureSuccessStatusCode();
		return results.Response.Files.Select(fi => new Abstract.File(fi.FileName, fi.ContentLength, fi.UploadTimestamp));
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