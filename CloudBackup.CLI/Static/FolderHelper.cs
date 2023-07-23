namespace CloudBackup.CLI.Static;

public enum EnumFilesResult
{
    Continue,
    NextFolder,
    Stop
}

public static class FolderHelper
{
	public static void EnumFiles(string path, Func<FileInfo, EnumFilesResult> onFileFound, string searchPattern = "*", Func<FileInfo, bool>? filter = null) =>
	    EnumFilesInner(path, searchPattern, onFileFound, filter);

    public static void EnumFolders(string path, Func<DirectoryInfo, EnumFilesResult> onFolderFound, string searchPattern = "*", Func<DirectoryInfo, bool>? filter = null) =>
        EnumFoldersInner(path, searchPattern, onFolderFound, filter);
        
    private static void EnumFoldersInner(string path, string searchPattern, Func<DirectoryInfo, EnumFilesResult> onFolderFound, Func<DirectoryInfo, bool>? filter)
	{
        var folders = TryGetDirectories(path, searchPattern);

        foreach (var folder in folders)
        {
            var info = new DirectoryInfo(folder);
            if (filter?.Invoke(info) ?? true)
            {
                var result = onFolderFound.Invoke(info);
                if (result == EnumFilesResult.Stop) return;
                if (result == EnumFilesResult.NextFolder) break;
                EnumFoldersInner(folder, searchPattern, onFolderFound, filter);
            }
        }
    }

    public static IEnumerable<string> TryGetDirectories(string path, string searchPattern)
    {
        try
        {
            return Directory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

	private static void EnumFilesInner(string path, string searchPattern, Func<FileInfo, EnumFilesResult> onFileFound, Func<FileInfo, bool>? filter = null)
	{
		var files = TryGetFiles(path, searchPattern);
		foreach (var file in files)
		{
			var info = new FileInfo(file);
			if (filter?.Invoke(info) ?? true)
			{
				var action = onFileFound.Invoke(info);
				if (action == EnumFilesResult.Stop) return;
				if (action == EnumFilesResult.NextFolder) break;
			}
		}

		var dirs = TryGetDirectories(path, "*");
		foreach (var dir in dirs) EnumFilesInner(dir, searchPattern, onFileFound, filter);
	}

	public static IEnumerable<string> TryGetFiles(string path, string searchPattern)
	{
		try
		{
			return Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}
		catch
		{
			return Enumerable.Empty<string>();
		}
	}
}