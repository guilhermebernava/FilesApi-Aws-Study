using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Files.Api.Aws.Interfaces;
using Files.Api.Models;

namespace Files.Api.Aws;

public class AuthServices : IAuthServices
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly string _userPoolId;
    private readonly string _appClientId;
    private readonly IAwsLogger _logger;

    public AuthServices(IConfiguration configuration, IAwsLogger logger)
    {
        var awsSettings = configuration.GetSection("AWS").Get<AwsSettingsModel>();
        _cognitoClient = new AmazonCognitoIdentityProviderClient(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region));
        _userPoolId = awsSettings.UserPoolId;
        _appClientId = awsSettings.AppClientId;
        _logger = logger;
    }

    public async Task<bool> ResetPasswordAsync(LoginModel model)
    {
        try
        {
            var authRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _userPoolId,
                ClientId = _appClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", model.Email },
                    { "PASSWORD", model.Password }
                }
            };

            var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest);

            if (authResponse.ChallengeName != "NEW_PASSWORD_REQUIRED") return false;

            var newPasswordRequest = new AdminSetUserPasswordRequest
            {
                UserPoolId = _userPoolId,
                Username = model.Email,
                Password = model.NewPassword,
                Permanent = true
            };

            var result = await _cognitoClient.AdminSetUserPasswordAsync(newPasswordRequest);

            return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            await _logger.Error(ex.Message, "ResetPasswordAsync");
            return false;
        }
    }


    public async Task<TokenModel?> LoginAsync(LoginModel model)
    {
        try
        {
            var authRequest = new AdminInitiateAuthRequest
            {
                UserPoolId = _userPoolId,
                ClientId = _appClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", model.Email },
                    { "PASSWORD", model.Password }
                }
            };

            var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest);

            if (authResponse.ChallengeName == "NEW_PASSWORD_REQUIRED") throw new Exception("Need to change person password");

            return new TokenModel()
            {
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken
            };
        }
        catch (Exception ex)
        {
            await _logger.Error(ex.Message, "LoginAsync");
            return null;
        }
    }
}
