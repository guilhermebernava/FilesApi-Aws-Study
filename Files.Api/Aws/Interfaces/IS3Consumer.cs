namespace Files.Api.Aws.Interfaces;

public interface IS3Consumer
{
    Task<string> Upload(IFormFile file, string keyName);
    Task<string> GetFile(string fileUrl);
}
