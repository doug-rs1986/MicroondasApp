using System.Text.Json;

namespace MicroondasApp.Application
{
    public class ErrorLogService
    {
        public const string LogFileOperacao = "errorlogs.json";
        public const string LogFileLogin = "errorlogs_login.json";
        private const int MaxLogs = 15;

        public void Registrar(ErrorLogEntry logEntry, string file)
        {
            var logs = Listar(file);
            logs.Insert(0, logEntry);
            if (logs.Count > MaxLogs)
                logs = logs.Take(MaxLogs).ToList();
            File.WriteAllText(file, JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true }));
        }

        public List<ErrorLogEntry> Listar(string file)
        {
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonSerializer.Deserialize<List<ErrorLogEntry>>(json) ?? new List<ErrorLogEntry>();
            }
            return new List<ErrorLogEntry>();
        }
    }
}
