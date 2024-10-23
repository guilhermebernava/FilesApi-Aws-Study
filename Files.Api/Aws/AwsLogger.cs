using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.Runtime;
using Files.Api.Aws.Interfaces;
using Files.Api.Models;

namespace Files.Api.Aws;

public class AwsLogger : IAwsLogger
{
    private readonly AmazonCloudWatchLogsClient _client;
    private readonly string _logGroup;
    private readonly string _logStream;
    public AwsLogger(IConfiguration configuration)
    {
        var awsSettings = configuration.GetSection("AWS").Get<AwsSettingsModel>();
        _logGroup = awsSettings.LogGroup;
        _logStream = awsSettings.LogStream;
        _client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region));
    }
    public async Task<bool> Error(string message, string serviceName)
    {
        var result = await _client.PutLogEventsAsync(new PutLogEventsRequest()
        {
            LogGroupName = _logGroup,
            LogStreamName = _logStream,
            LogEvents = new List<InputLogEvent>()
            {
                new InputLogEvent() {
                    Message = serviceName +" - "+ message,
                    Timestamp = DateTime.UtcNow,

                }
            }
        });

        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}
