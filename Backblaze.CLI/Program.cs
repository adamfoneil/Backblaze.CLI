using System.Text.Json;

var settings = FindSettings();

Console.WriteLine($"name = {settings.Name}");
Console.ReadLine();

Backblaze.CLI.Models.Backblaze FindSettings()
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
	return JsonSerializer.Deserialize<Backblaze.CLI.Models.Backblaze>(json) ?? throw new Exception("Couldn't deserialize json");
}