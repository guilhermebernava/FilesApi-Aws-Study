namespace Files.Api.Aws.Interfaces;

public interface IAwsLogger
{
    Task<bool> Error(string message,string serviceName);
}
