var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/upload", IResult (IFormFile file) =>
{
    long maxFileSize = 5 * 1024 * 1024;
    if (file == null || file.Length == 0)return  Results.BadRequest("No found any file");
    if (file.Length > maxFileSize) return Results.BadRequest("Olny file less THAN 5MB is accepted");
   
    return Results.Ok(new { Message = "Archive has been uploaded" });
})
.WithOpenApi();

app.Run();
