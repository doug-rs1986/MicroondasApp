using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MicroondasApp.Application;

namespace MicroondasApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly IConfiguration _config;
        private readonly ErrorLogService _errorLogService;

        public AuthController(UsuarioService usuarioService, IConfiguration config, ErrorLogService errorLogService)
        {
            _usuarioService = usuarioService;
            _config = config;
            _errorLogService = errorLogService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Nome) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest("Nome e senha são obrigatórios.");
            try
            {
                _usuarioService.Criar(req.Nome, req.Senha);
                return Ok("Usuário cadastrado com sucesso.");
            }
            catch (Exception ex)
            {
                _errorLogService.Registrar(new MicroondasApp.Application.ErrorLogEntry {
                    Data = DateTime.Now,
                    Mensagem = ex.Message,
                    Tipo = ex.GetType().Name,
                    Caminho = "auth/register",
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.ToString()
                }, ErrorLogService.LogFileLogin);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest req)
        {
            try
            {
                if (!_usuarioService.ValidarSenha(req.Nome, req.Senha))
                {
                    _errorLogService.Registrar(new MicroondasApp.Application.ErrorLogEntry {
                        Data = DateTime.Now,
                        Mensagem = "Usuário ou senha inválidos.",
                        Tipo = "LoginError",
                        Caminho = "auth/login"
                    }, ErrorLogService.LogFileLogin);
                    return Unauthorized("Usuário ou senha inválidos.");
                }
                var usuario = _usuarioService.ObterPorNome(req.Nome);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario!.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Nome)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds
                );
                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            catch (Exception ex)
            {
                _errorLogService.Registrar(new MicroondasApp.Application.ErrorLogEntry {
                    Data = DateTime.Now,
                    Mensagem = ex.Message,
                    Tipo = ex.GetType().Name,
                    Caminho = "auth/login",
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.ToString()
                }, ErrorLogService.LogFileLogin);
                return StatusCode(500, "Erro inesperado ao realizar login.");
            }
        }

        [HttpGet("status")]
        public ActionResult Status()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                    return Ok("Autenticado");
                _errorLogService.Registrar(new MicroondasApp.Application.ErrorLogEntry {
                    Data = DateTime.Now,
                    Mensagem = "Usuário não autenticado.",
                    Tipo = "AuthStatusError",
                    Caminho = "auth/status"
                }, ErrorLogService.LogFileLogin);
                return Unauthorized("Não autenticado");
            }
            catch (Exception ex)
            {
                _errorLogService.Registrar(new MicroondasApp.Application.ErrorLogEntry {
                    Data = DateTime.Now,
                    Mensagem = ex.Message,
                    Tipo = ex.GetType().Name,
                    Caminho = "auth/status",
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.ToString()
                }, ErrorLogService.LogFileLogin);
                return StatusCode(500, "Erro inesperado ao verificar status de autenticação.");
            }
        }

        public class RegisterRequest
        {
            public string Nome { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
        public class LoginRequest
        {
            public string Nome { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
    }
}
