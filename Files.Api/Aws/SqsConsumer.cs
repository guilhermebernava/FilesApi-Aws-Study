using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Files.Api.Aws.Interfaces;
using Files.Api.Aws.Models;
using System.Net;
using System.Text.Json;

namespace Files.Api.Aws;

public class SqsConsumer : ISqsConsumer
{
    private readonly IAmazonSQS _amazonSQS;
    private readonly string _queueUrl;

    public SqsConsumer(IConfiguration configuration)
    {
        var awsSettings = configuration.GetSection("AWS").Get<AwsSettingsModel>();
        _amazonSQS = new AmazonSQSClient(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region));
        _queueUrl = awsSettings.QueueFileUrl;
    }
    public async Task<Object> GetMessage(IS3Consumer s3Consumer)
    {
        var response = await _amazonSQS.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 20
        });

        var message = response.Messages.FirstOrDefault();
        if (message == null) return null;

        var body = JsonSerializer.Deserialize<MessageModel>(message.Body);
        var file = s3Consumer.GetFile(body.FileUrl);
        await _amazonSQS.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);

        return response;
    }

    public async Task<bool> SendMessage(string message)
    {
        var sended = await _amazonSQS.SendMessageAsync(_queueUrl, message);
        return sended.HttpStatusCode == HttpStatusCode.OK;
    }
}
