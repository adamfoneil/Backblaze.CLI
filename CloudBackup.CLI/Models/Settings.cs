using System.Text.Json.Serialization;

namespace CloudBackup.CLI.Models;

public class Settings
{
	/// <summary>
	/// root folder of all your backups
	/// </summary>
	public string BaseFolder { get; set; } = default!;
	/// <summary>
	/// local folders you want to include in backup
	/// </summary>
	public string[] Sources { get; set; } = Array.Empty<string>();
	[JsonPropertyName("Backblaze")]
	public BackblazeSettings BackblazeSettings { get; set; } = new();
}
