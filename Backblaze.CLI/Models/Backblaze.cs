using Backblaze.CLI.Interfaces;

namespace Backblaze.CLI.Models;

public class Backblaze : ICloudTarget
{
	public string Name { get; set; } = default!;
	public string BucketName { get; set; } = default!;
	public string PublicKey { get; set; } = default!;
	public string SecretKey { get; set; } = default!;

	public string Type => "Backblaze";
}
