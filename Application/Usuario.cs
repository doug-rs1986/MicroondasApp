using System.Text.Json.Serialization;

namespace MicroondasApp.Application
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        [JsonIgnore]
        public string Senha { get; set; } = string.Empty; // Armazenada já criptografada SHA256
    }
}
