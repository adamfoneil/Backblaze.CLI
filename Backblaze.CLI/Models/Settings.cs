using Backblaze.CLI.Interfaces;

namespace Backblaze.CLI.Models;

public class Settings
{
	public string BaseFolder { get; set; } = default!;
	public string[] Sources { get; set; } = Array.Empty<string>();
	public ICloudTarget[] Targets { get; set; } = Array.Empty<ICloudTarget>();
}
