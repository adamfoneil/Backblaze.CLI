using CloudBackup.CLI.Abstract;
using Microsoft.Extensions.Logging;

namespace CloudBackup.CLI.Providers;

public class LocalProviderSettings
{
	public string TargetFolder { get; set; } = default!;
}

public class LocalProvider : BackupProvider<LocalProviderSettings>
{
    public LocalProvider(string baseFolder, LocalProviderSettings settings, ILogger logger) : base(baseFolder, settings, logger)
    {            
    }

	public override string Type => "Local";

	protected override Task<IEnumerable<Abstract.File>> GetCloudFilesAsync(string folder)
	{
		throw new NotImplementedException();
	}

	protected override Task UploadFileAsync(Abstract.File file)
	{
		throw new NotImplementedException();
	}
}
