using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using MicroondasApp.Domain;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microondas API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = exceptionHandlerPathFeature?.Error;
        var response = new {
            Sucesso = false,
            Mensagem = ex is RegrasDeNegocioException ? ex.Message : "Ocorreu um erro inesperado.",
            Tipo = ex?.GetType().Name,
            Caminho = exceptionHandlerPathFeature?.Path
        };
        // Log de exceptions não tratadas
        if (ex is not RegrasDeNegocioException)
        {
            var log = $"Data: {DateTime.Now}\nException: {ex}\nInnerException: {ex?.InnerException}\nStackTrace: {ex?.StackTrace}\nPath: {exceptionHandlerPathFeature?.Path}\n---\n";
            File.AppendAllText("exceptions.log", log);
        }
        context.Response.StatusCode = ex is RegrasDeNegocioException ? 400 : 500;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microondas API v1");
    c.RoutePrefix = string.Empty;
});

app.Run();
