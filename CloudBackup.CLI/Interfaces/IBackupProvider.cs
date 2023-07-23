using CloudBackup.CLI.Abstract;

namespace CloudBackup.CLI.Interfaces;

public interface IBackupProvider
{
    string Type { get; }
    Task ExecuteAsync(IEnumerable<string> folders, IProgress<Progress>? progress = null);
    Task<IEnumerable<Abstract.File>> GetLocalChangesAsync(IDisposable? client, string folder);
}