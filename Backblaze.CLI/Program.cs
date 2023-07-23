using System.Text.Json;

var settings = FindSettings();

Console.WriteLine($"name = {settings.Name}");
Console.ReadLine();

CloudBackup.CLI.Models.BackblazeSettings FindSettings()
{
	const string fileName = "backblaze.json";
	var baseFolder = AppContext.BaseDirectory;
	var configFile = Path.Combine(baseFolder, fileName);

	while (!File.Exists(configFile))
	{
		var folder = Path.GetDirectoryName(configFile) ?? throw new Exception($"Couldn't get directory name from {configFile}");
		baseFolder = Directory.GetParent(folder)?.FullName ?? throw new Exception($"Couldn't get parent directory of {folder}");
		configFile = Path.Combine(baseFolder, fileName);
	}

	var json = File.ReadAllText(configFile);
	return JsonSerializer.Deserialize<CloudBackup.CLI.Models.BackblazeSettings>(json) ?? throw new Exception("Couldn't deserialize json");
}