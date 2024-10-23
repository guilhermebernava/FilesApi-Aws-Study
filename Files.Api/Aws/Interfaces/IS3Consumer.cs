namespace Files.Api.Aws.Interfaces;

public interface IS3Consumer
{
    Task<string> UploadAsync(IFormFile file, string keyName);
    Task<string> GetFileAsync(string fileUrl);
}
