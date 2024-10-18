namespace Files.Api.Aws.Models;

public class AwsSettingsModel
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public string BucketName { get; set; }
    public string QueueFileUrl { get; set; }
}
