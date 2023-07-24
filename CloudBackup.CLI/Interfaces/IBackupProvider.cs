using CloudBackup.CLI.Abstract;

namespace CloudBackup.CLI.Interfaces;

/// <summary>
/// this is for runtime dispatcing in Main method if we have multiple providers created
/// </summary>
public interface IBackupProvider
{
    string Type { get; }
    Task ExecuteAsync(IEnumerable<string> folders, IProgress<Progress>? progress = null);
    Task<IEnumerable<Abstract.File>> GetLocalChangesAsync(IDisposable? client, string folder);
}