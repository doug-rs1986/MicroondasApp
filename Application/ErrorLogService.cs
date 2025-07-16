using System.Text.Json;

namespace MicroondasApp.Application
{
    public class ErrorLogService
    {
        private const string LogFile = "errorlogs.json";
        private const int MaxLogs = 15;

        public void Registrar(ErrorLogEntry logEntry)
        {
            var logs = Listar();
            logs.Insert(0, logEntry);
            if (logs.Count > MaxLogs)
                logs = logs.Take(MaxLogs).ToList();
            File.WriteAllText(LogFile, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
        }

        public List<ErrorLogEntry> Listar()
        {
            if (File.Exists(LogFile))
            {
                var json = File.ReadAllText(LogFile);
                return JsonSerializer.Deserialize<List<ErrorLogEntry>>(json) ?? new List<ErrorLogEntry>();
            }
            return new List<ErrorLogEntry>();
        }
    }
}
