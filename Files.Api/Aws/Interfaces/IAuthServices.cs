using Files.Api.Models;

namespace Files.Api.Aws.Interfaces;

public interface IAuthServices
{
    Task<TokenModel?> LoginAsync(LoginModel model);
    Task<bool> ResetPasswordAsync(LoginModel model);
}
