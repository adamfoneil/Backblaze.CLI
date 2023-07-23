namespace CloudBackup.CLI.Interfaces;

public interface IBackupProvider
{
    string Type { get; }
    Task ExecuteAsync(IEnumerable<string> folders);
    Task<IEnumerable<Abstract.File>> GetLocalChangesAsync(IDisposable? client, string folder);
}