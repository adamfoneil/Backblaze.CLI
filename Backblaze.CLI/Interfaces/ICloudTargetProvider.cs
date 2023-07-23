using CloudBackup.CLI.Abstract;

namespace CloudBackup.CLI.Interfaces;

public interface ICloudTargetProvider<TConfig> where TConfig : ICloudTargetSettings
{
    Task ExecuteAsync(IEnumerable<string> sources);
    Task<IEnumerable<CloudBackupTarget<TConfig>.File>> GetLocalChangesAsync(string folder);
}