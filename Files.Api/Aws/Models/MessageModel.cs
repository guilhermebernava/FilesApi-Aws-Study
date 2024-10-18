namespace Files.Api.Aws.Models;

public class MessageModel
{
    public MessageModel(string fileName, string fileUrl)
    {
        FileName = fileName;
        FileUrl = fileUrl;
    }

    public string FileName { get; set; }
    public string FileUrl { get; set; }
}
