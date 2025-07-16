using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MicroondasApp.Domain;
using MicroondasApp.Application;

namespace MicroondasApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                var path = context.Request.Path;
                var response = new {
                    Sucesso = false,
                    Mensagem = ex is RegrasDeNegocioException ? ex.Message : "Ocorreu um erro inesperado.",
                    Tipo = ex.GetType().Name,
                    Caminho = path
                };
                if (ex is not RegrasDeNegocioException)
                {
                    var logEntry = new ErrorLogEntry {
                        Data = DateTime.Now,
                        Mensagem = ex.Message,
                        Tipo = ex.GetType().Name,
                        Caminho = path,
                        StackTrace = ex.StackTrace,
                        InnerException = ex.InnerException?.ToString()
                    };
                    var logService = context.RequestServices.GetRequiredService<ErrorLogService>();
                    logService.Registrar(logEntry, ErrorLogService.LogFileOperacao);
                }
                context.Response.StatusCode = ex is RegrasDeNegocioException ? 400 : 500;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
