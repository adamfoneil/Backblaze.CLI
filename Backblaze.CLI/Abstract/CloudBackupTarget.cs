using Backblaze.CLI.Interfaces;
using Backblaze.CLI.Static;
using Microsoft.Extensions.Logging;

namespace Backblaze.CLI.Abstract;

public abstract class CloudBackupTarget<TConfig> where TConfig : ICloudTarget
{
    private readonly string BaseFolder;    
    private readonly ILogger<CloudBackupTarget<TConfig>> Logger;

    public CloudBackupTarget(string baseFolder, ILogger<CloudBackupTarget<TConfig>> logger)
    {
        BaseFolder = baseFolder;
        Logger = logger;
    }

    public async Task<IEnumerable<File>> QueryChangesAsync(string folder)
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
			var changes = await QueryChangesAsync(source);
            foreach (var change in changes)
            {
                await UploadFileAsync(change);
                Logger.LogInformation("{@file}", change);
            }
		}        
    }

    public record File(string Path, long Length, DateTime DateModified) { }
}
