using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MicroondasApp.Application
{
    public class UsuarioService
    {
        private const string UsuariosFile = "usuarios.json";
        private static List<Usuario>? _usuarios;

        public List<Usuario> Listar()
        {
            if (_usuarios != null) return _usuarios;
            if (File.Exists(UsuariosFile))
            {
                var json = File.ReadAllText(UsuariosFile);
                _usuarios = JsonSerializer.Deserialize<List<Usuario>>(json) ?? new List<Usuario>();
            }
            else
            {
                _usuarios = new List<Usuario>();
            }
            return _usuarios;
        }

        public void Salvar()
        {
            File.WriteAllText(UsuariosFile, JsonSerializer.Serialize(_usuarios, new JsonSerializerOptions { WriteIndented = true }));
        }

        public Usuario? ObterPorNome(string nome)
        {
            return Listar().FirstOrDefault(u => u.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        }

        public Usuario? ObterPorId(int id)
        {
            return Listar().FirstOrDefault(u => u.Id == id);
        }

        public bool NomeExiste(string nome) => Listar().Any(u => u.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

        public Usuario Criar(string nome, string senha)
        {
            if (NomeExiste(nome)) throw new Exception("Nome de usuário já existe.");
            var usuario = new Usuario
            {
                Id = Listar().Count > 0 ? Listar().Max(u => u.Id) + 1 : 1,
                Nome = nome,
                Senha = HashSenha(senha)
            };
            _usuarios!.Add(usuario);
            Salvar();
            return usuario;
        }

        public bool ValidarSenha(string nome, string senha)
        {
            var usuario = ObterPorNome(nome);
            if (usuario == null) return false;
            return usuario.Senha == HashSenha(senha);
        }

        public static string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
