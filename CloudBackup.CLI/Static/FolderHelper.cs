namespace CloudBackup.CLI.Static;

public enum EnumFilesResult
{
    Continue,
    NextFolder,
    Stop
}

public static class FolderHelper
{
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
}