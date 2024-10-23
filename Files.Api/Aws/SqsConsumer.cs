using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Files.Api.Aws.Interfaces;
using Files.Api.Models;
using System.Net;
using System.Text.Json;

namespace Files.Api.Aws;

public class SqsConsumer : ISqsConsumer
{
    private readonly IAmazonSQS _amazonSQS;
    private readonly string _queueUrl;
    private readonly IAwsLogger _logger;

    public SqsConsumer(IConfiguration configuration, IAwsLogger logger)
    {
        var awsSettings = configuration.GetSection("AWS").Get<AwsSettingsModel>();
        _amazonSQS = new AmazonSQSClient(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region));
        _queueUrl = awsSettings.QueueFileUrl;
        _logger = logger;
    }
    public async Task<Object> GetMessageAsync(IS3Consumer s3Consumer)
    {
        try
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
            var file = s3Consumer.GetFileAsync(body.FileUrl);
            await _amazonSQS.DeleteMessageAsync(_queueUrl, message.ReceiptHandle);

            return response;
        }
        catch (Exception ex)
        {
            await _logger.Error(ex.Message, "GetMessageAsync");
            return null;
        }     
    }

    public async Task<bool> SendMessageAsync(string message)
    {
        try
        {
            var sended = await _amazonSQS.SendMessageAsync(_queueUrl, message);
            return sended.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            await _logger.Error(ex.Message, "SendMessageAsync");
            return false;
        }
    }
}
