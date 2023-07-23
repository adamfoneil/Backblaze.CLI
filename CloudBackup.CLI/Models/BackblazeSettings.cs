namespace CloudBackup.CLI.Models;

public class BackblazeSettings
{
	public string AccountName { get; set; } = default!;
	public string BucketName { get; set; } = default!;
	public string PublicKey { get; set; } = default!;
	public string SecretKey { get; set; } = default!;
}
