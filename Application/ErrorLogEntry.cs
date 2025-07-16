namespace MicroondasApp.Application
{
    public class ErrorLogEntry
    {
        public DateTime Data { get; set; }
        public string? Mensagem { get; set; }
        public string? Tipo { get; set; }
        public string? Caminho { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
    }
}
