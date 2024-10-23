using Files.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Files.Api.Injections;

public static class JwtInjector
{
    public static void AddJWTConf(this WebApplicationBuilder builder)
    {
        var awsSettings = builder.Configuration.GetSection("AWS").Get<AwsSettingsModel>();
        string issuer = $"https://cognito-idp.{awsSettings.Region}.amazonaws.com/{awsSettings.UserPoolId}";

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = issuer;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = issuer,
            };
        });
    }
}
