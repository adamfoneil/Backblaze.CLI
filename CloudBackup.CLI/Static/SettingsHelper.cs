using CloudBackup.CLI.Models;
using System.Text.Json;

namespace CloudBackup.CLI.Static;

public static class SettingsHelper
{
	public static Settings GetSettings(string fileName) => GetSettings(new[] { fileName });

	public static Settings GetSettings(string[] args)
	{
		if (args.Length == 0) throw new ArgumentNullException("Settings file parameter expected");
		
		var fileName = args[0];
		if (!File.Exists(fileName)) throw new FileNotFoundException($"Settings file not found: {fileName}");

		var json = File.ReadAllText(fileName);
		return JsonSerializer.Deserialize<Settings>(json) ?? throw new ArgumentException("Couldn't deserialize settings json");
	}
}
