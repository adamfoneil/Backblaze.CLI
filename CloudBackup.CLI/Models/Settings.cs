using System.Text.Json.Serialization;

namespace CloudBackup.CLI.Models;

public class Settings
{
	public string BaseFolder { get; set; } = default!;
	public string[] Sources { get; set; } = Array.Empty<string>();
	[JsonPropertyName("Backblaze")]
	public BackblazeSettings BackblazeSettings { get; set; } = new();
}
