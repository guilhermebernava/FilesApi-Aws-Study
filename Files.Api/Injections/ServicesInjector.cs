using Files.Api.Aws.Interfaces;
using Files.Api.Aws;

namespace Files.Api.Injections;

public static class ServicesInjector
{
    public static void AddAwsServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAwsLogger, AwsLogger>();


        builder.Services.AddSingleton<IS3Consumer, S3Consumer>();
        builder.Services.AddSingleton<ISqsConsumer, SqsConsumer>();
        builder.Services.AddSingleton<IAuthServices, AuthServices>();
    }

}
