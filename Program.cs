using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using MicroondasApp.Domain;
using MicroondasApp.Application;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microondas API", Version = "v1" });
});

// Registro dos serviços no DI
builder.Services.AddSingleton<MicroondasStateService>();
builder.Services.AddSingleton<ErrorLogService>();
builder.Services.AddTransient<MicroondasService>();

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
            var logEntry = new MicroondasApp.Application.ErrorLogEntry {
                Data = DateTime.Now,
                Mensagem = ex?.Message,
                Tipo = ex?.GetType().Name,
                Caminho = exceptionHandlerPathFeature?.Path,
                StackTrace = ex?.StackTrace,
                InnerException = ex?.InnerException?.ToString()
            };
            var logService = context.RequestServices.GetRequiredService<ErrorLogService>();
            logService.Registrar(logEntry);
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

public class ErrorLogEntry
{
    public DateTime Data { get; set; }
    public string? Mensagem { get; set; }
    public string? Tipo { get; set; }
    public string? Caminho { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}
