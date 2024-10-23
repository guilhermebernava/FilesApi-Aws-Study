using Files.Api.Aws;
using Files.Api.Aws.Interfaces;
using Files.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Files.Api.Injections;

public static class EndpointInjector
{
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapGet("/test", async ([FromServices]IAwsLogger logger) =>
        {
            return await logger.Error("teste", "/test");
        });

        //TODO fazer endpoint para baixar arquivos do S3

        app.MapPost("/resetPassword", async Task<IResult> ([FromServices] IAuthServices cognitoConsumer, [FromBody] LoginModel model) =>
        {
            var result = await cognitoConsumer.ResetPasswordAsync(model);
            if (!result) return TypedResults.BadRequest("Error in reset password");

            return TypedResults.Ok(result);
        });

        app.MapPost("/login", async Task<IResult> ([FromServices] IAuthServices cognitoConsumer, [FromBody] LoginModel model) =>
        {
            var result = await cognitoConsumer.LoginAsync(model);
            if (result == null) return TypedResults.BadRequest("Error in login");

            return TypedResults.Ok(result);
        });

        app.MapGet("/message", [Authorize] async Task<IResult> ([FromServices] ISqsConsumer sqsConsumer, [FromServices] IS3Consumer s3Consumer) =>
        {
            var messages = await sqsConsumer.GetMessageAsync(s3Consumer);
            return TypedResults.Ok(messages);
        });

        app.MapPost("/upload", [Authorize] async Task<IResult> ([FromServices] IS3Consumer s3Consumer, [FromServices] ISqsConsumer sqsConsumer, IFormFile file) =>
        {
            long maxFileSize = 5 * 1024 * 1024;
            if (file == null || file.Length == 0) return TypedResults.BadRequest("No found any file");
            if (file.Length > maxFileSize) return TypedResults.BadRequest("Olny file less THAN 5MB is accepted");

            var fileName = Guid.NewGuid().ToString();
            var fileUrl = await s3Consumer.UploadAsync(file, fileName);

            if (fileUrl == "") return Results.BadRequest("Error in upload file");
            var message = new MessageModel(fileName, fileUrl);

            await sqsConsumer.SendMessageAsync(JsonSerializer.Serialize(message));
            return Results.Ok(new { Message = "Sended to queue" });
        }).DisableAntiforgery();
    }
}
