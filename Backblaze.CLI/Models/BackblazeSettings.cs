using CloudBackup.CLI.Interfaces;

namespace CloudBackup.CLI.Models;

public class BackblazeSettings : ICloudTargetSettings
{
	public string Name { get; set; } = default!;
	public string BucketName { get; set; } = default!;
	public string PublicKey { get; set; } = default!;
	public string SecretKey { get; set; } = default!;

	public string Type => "Backblaze";
}
