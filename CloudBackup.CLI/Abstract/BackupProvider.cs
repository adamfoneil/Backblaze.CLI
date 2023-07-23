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
		var cloudFiles = (await GetCloudFilesAsync(client, folder)).ToDictionary(item => item.Path);
		var localFiles = GetLocalFiles(Path.Combine(BaseFolder, folder));
		return localFiles.Where(file => IsNewOrModified(file, cloudFiles));
	}

	private bool IsNewOrModified(File localFile, Dictionary<string, File> cloudFiles)
	{
		if (cloudFiles.TryGetValue(localFile.Path, out var cloudFile))
		{
			if (cloudFile.DateModified > localFile.DateModified) return false;
		}

		return true;
	}

	private IEnumerable<File> GetLocalFiles(string folder)
	{
		List<File> results = new();

		FolderHelper.EnumFiles(folder, (fileInfo) =>
		{
			results.Add(new File(fileInfo.FullName.Substring(BaseFolder.Length + 1), fileInfo.Length, fileInfo.LastWriteTimeUtc));
			return EnumFilesResult.Continue;
		});

		return results;
	}

	protected virtual bool HasInternalClient => true;

	protected abstract Task<IDisposable> GetClientAsync();

	protected abstract Task<IEnumerable<File>> GetCloudFilesAsync(IDisposable? client, string folder);

	protected abstract Task UploadFileAsync(IDisposable? client, string basePath, File file);

	public async Task ExecuteAsync(IEnumerable<string> folders, IProgress<Progress>? progress = null)
	{
		using var client = (HasInternalClient) ? await GetClientAsync() : default;

		foreach (var source in folders)
		{
			var changes = (await GetLocalChangesAsync(client, source)).ToArray();
			int total = changes.Length;
			int count = 0;
			foreach (var change in changes)
			{
				count++;
				await UploadFileAsync(client, BaseFolder, change);
				progress?.Report(new(source, count / (decimal)total));
				Logger.LogInformation("{@file}", change);
			}
		}
	}	
}

public record File(string Path, long Length, DateTime DateModified) { }

public record Progress(string Folder, decimal Percent) { }