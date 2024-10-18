using Files.Api.Aws;
using Files.Api.Aws.Interfaces;
using Files.Api.Aws.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IS3Consumer,S3Consumer>();
builder.Services.AddSingleton<ISqsConsumer,SqsConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/message", async Task<IResult> ([FromServices] ISqsConsumer sqsConsumer, [FromServices] IS3Consumer s3Consumer) =>
{
    var messages = await sqsConsumer.GetMessage(s3Consumer);
    return TypedResults.Ok(messages);
});

app.MapPost("/upload", async Task<IResult> ([FromServices] IS3Consumer s3Consumer, [FromServices] ISqsConsumer sqsConsumer, IFormFile file) =>
{
    long maxFileSize = 5 * 1024 * 1024;
    if (file == null || file.Length == 0) return TypedResults.BadRequest("No found any file");
    if (file.Length > maxFileSize) return TypedResults.BadRequest("Olny file less THAN 5MB is accepted");

    var fileName = Guid.NewGuid().ToString();
    var fileUrl = await s3Consumer.Upload(file, fileName);

    if (fileUrl == "") return Results.BadRequest("Error in upload file");
    var message = new MessageModel(fileName, fileUrl);

    await sqsConsumer.SendMessage(JsonSerializer.Serialize(message));
    return Results.Ok(new { Message = "Sended to queue" });
}).DisableAntiforgery();

app.Run();
