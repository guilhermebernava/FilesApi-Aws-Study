using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Files.Api.Aws.Interfaces;
using Files.Api.Aws.Models;

namespace Files.Api.Aws;

public class S3Consumer : IS3Consumer
{
    private readonly IAmazonS3 _client;
    private readonly string _bucketName;

    public S3Consumer(IConfiguration configuration)
    {
        var awsSettings = configuration.GetSection("AWS").Get<AwsSettingsModel>();
        _client = new AmazonS3Client(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region));
        _bucketName = awsSettings.BucketName;
    }

    public async Task<string> GetFile(string fileUrl)
    {
        var s3Uri = new Uri(fileUrl);
        var key = s3Uri.AbsolutePath.TrimStart('/');

        var getRequest = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        using (var response = await _client.GetObjectAsync(getRequest))
        using (var responseStream = response.ResponseStream)
        using (var reader = new StreamReader(responseStream))
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var deleted = await _client.DeleteObjectAsync(deleteRequest);
            return await reader.ReadToEndAsync();
        }
    }

    public async Task<string> Upload(IFormFile file, string keyName)
    {
        using (var stream = file.OpenReadStream())
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = keyName,
                InputStream = stream,
                ContentType = file.ContentType
            };

            var response = await _client.PutObjectAsync(putRequest);

            if (((int)response.HttpStatusCode) != 200) return "";

            return $"https://{_bucketName}.s3.amazonaws.com/{keyName}";
        }
    }
}
