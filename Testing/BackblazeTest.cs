using CloudBackup.CLI.Providers;
using CloudBackup.CLI.Static;
using Microsoft.Extensions.Logging;

namespace Testing;

[TestClass]
public class BackblazeTest
{
	[TestMethod]
	public async Task BackblazeProvider()
	{
		var settings = SettingsHelper.GetSettings("backup-cli.json");
		var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger<BackblazeTest>();

		var provider = new BackblazeBackupProvider(settings.BaseFolder, settings.BackblazeSettings, logger);
		await provider.ExecuteAsync(settings.Sources);
	}
}