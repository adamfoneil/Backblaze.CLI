using CloudBackup.CLI.Interfaces;
using CloudBackup.CLI.Models;
using CloudBackup.CLI.Providers;
using Microsoft.Extensions.Logging;
using System.Text.Json;

if (args.Length == 0) throw new ArgumentNullException("Settings file parameter expected");

var settingsFile = args[0];
if (!File.Exists(settingsFile)) throw new FileNotFoundException($"Settings file not found: {settingsFile}");

var json = File.ReadAllText(settingsFile);
var settings = JsonSerializer.Deserialize<Settings>(json) ?? throw new ArgumentException("Couldn't deserialize settings json");


Dictionary<string, IBackupProvider> Providers(string baseFolder, LoggerFactory loggerFactory) => 
	new IBackupProvider[]
	{
		new BackblazeBackupProvider(baseFolder, settings!.BackblazeSettings, loggerFactory.CreateLogger("Backblaze"))
	}.ToDictionary(item => item.Type);
