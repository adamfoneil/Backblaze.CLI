using CloudBackup.CLI.Interfaces;
using CloudBackup.CLI.Providers;
using CloudBackup.CLI.Static;
using Microsoft.Extensions.Logging;



var settings = SettingsHelper.GetSettings(args);


Dictionary<string, IBackupProvider> Providers(string baseFolder, LoggerFactory loggerFactory) => 
	new IBackupProvider[]
	{
		new BackblazeBackupProvider(baseFolder, settings!.BackblazeSettings, loggerFactory.CreateLogger("Backblaze"))
	}.ToDictionary(item => item.Type);
