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

	protected override bool HasInternalClient => false;

	protected override Task<IDisposable> GetClientAsync() => throw new NotImplementedException();

	protected override Task<IEnumerable<Abstract.File>> GetCloudFilesAsync(IDisposable? client, string folder)
	{
		throw new NotImplementedException();
	}

	protected override Task UploadFileAsync(IDisposable? client, string basePath, Abstract.File file)
	{
		throw new NotImplementedException();
	}
}
