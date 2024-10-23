namespace Files.Api.Aws.Interfaces;

public interface ISqsConsumer
{
    Task<Object> GetMessageAsync(IS3Consumer s3Consumer);
    Task<bool> SendMessageAsync(string message);
}
