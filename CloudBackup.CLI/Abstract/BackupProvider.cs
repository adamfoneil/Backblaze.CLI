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

	public async Task<IEnumerable<File>> GetLocalChangesAsync(string folder)
	{
		var cloudFiles = await GetCloudFilesAsync(folder);
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

	protected abstract Task<IEnumerable<File>> GetCloudFilesAsync(string folder);

	protected abstract Task UploadFileAsync(File file);

	public async Task ExecuteAsync(IEnumerable<string> sources)
	{
		foreach (var source in sources)
		{
			var changes = await GetLocalChangesAsync(source);
			foreach (var change in changes)
			{
				await UploadFileAsync(change);
				Logger.LogInformation("{@file}", change);
			}
		}
	}	
}

public record File(string Path, long Length, DateTime DateModified) { }