using CloudBackup.CLI.Abstract;
using CloudBackup.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CloudBackup.CLI.Providers;

internal class BackblazeBackupProvider : BackupProvider<BackblazeSettings>
{
    public BackblazeBackupProvider(string baseFolder, BackblazeSettings settings, ILogger logger) : base(baseFolder, settings, logger)
    {			
    }

    public override string Type => "Backblaze";

	protected override Task<IEnumerable<Abstract.File>> GetCloudFilesAsync(string folder)
	{
		throw new NotImplementedException();
	}

	protected override Task UploadFileAsync(Abstract.File file)
	{
		throw new NotImplementedException();
	}
}
