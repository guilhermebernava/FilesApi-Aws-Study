namespace Files.Api.Aws.Interfaces;

public interface ISqsConsumer
{
    Task<Object> GetMessage(IS3Consumer s3Consumer);
    Task<bool> SendMessage(string message);
}
