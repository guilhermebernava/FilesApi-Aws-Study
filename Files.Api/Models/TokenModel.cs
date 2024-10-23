namespace Files.Api.Models;

public class TokenModel
{
    public string AccessToken { get; set; }
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
}
