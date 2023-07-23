using CloudBackup.CLI.Interfaces;

namespace CloudBackup.CLI.Models;

public class Settings
{
	public string BaseFolder { get; set; } = default!;
	public string[] Sources { get; set; } = Array.Empty<string>();
	public ICloudTargetSettings[] Targets { get; set; } = Array.Empty<ICloudTargetSettings>();
}
