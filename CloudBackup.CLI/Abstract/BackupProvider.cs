using CloudBackup.CLI.Interfaces;
using CloudBackup.CLI.Static;
using Microsoft.Extensions.Logging;

namespace CloudBackup.CLI.Abstract;

public abstract class BackupProvider<TSettings> : IBackupProvider
{
	protected readonly string BaseFolder;
	protected readonly ILogger Logger;
	protected readonly TSettings Settings;

	public BackupProvider(string baseFolder, TSettings settings, ILogger logger)
	{
		BaseFolder = baseFolder;
		Settings = settings;
		Logger = logger;
	}

	public abstract string Type { get; }

	public async Task<IEnumerable<File>> GetLocalChangesAsync(IDisposable? client, string folder)
	{		
		var cloudFiles = await GetCloudFilesAsync(client, folder);
		var localFiles = GetLocalFiles(Path.Combine(BaseFolder, folder));
		return localFiles.Except(cloudFiles);
	}

	private IEnumerable<File> GetLocalFiles(string folder)
	{
		List<File> results = new();

		FolderHelper.EnumFolders(folder, (dir) =>
		{
			var files = dir.GetFiles();
			results.AddRange(files.Select(fi => new File(fi.FullName.Substring(BaseFolder.Length + 1), fi.Length, fi.LastWriteTimeUtc)));
			return EnumFilesResult.Continue;
		});

		return results;
	}

	protected virtual bool HasInternalClient => true;

	protected abstract Task<IDisposable> GetClientAsync();

	protected abstract Task<IEnumerable<File>> GetCloudFilesAsync(IDisposable? client, string folder);

	protected abstract Task UploadFileAsync(IDisposable? client, string basePath, File file);

	public async Task ExecuteAsync(IEnumerable<string> folders)
	{
		using var client = (HasInternalClient) ? await GetClientAsync() : default;

		foreach (var source in folders)
		{
			var changes = await GetLocalChangesAsync(client, source);
			foreach (var change in changes)
			{
				await UploadFileAsync(client, BaseFolder, change);
				Logger.LogInformation("{@file}", change);
			}
		}
	}	
}

public record File(string Path, long Length, DateTime DateModified) { }