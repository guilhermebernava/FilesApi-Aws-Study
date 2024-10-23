namespace Files.Api.Models;

public class AwsSettingsModel
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public string BucketName { get; set; }
    public string QueueFileUrl { get; set; }
    public string UserPoolId { get; set; }
    public string AppClientId { get; set; }
    public string LogGroup { get; set; }
    public string LogStream{ get; set; }
}
