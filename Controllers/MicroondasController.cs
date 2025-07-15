using Microsoft.AspNetCore.Mvc;
using MicroondasApp.Application;

namespace MicroondasApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroondasController : ControllerBase
    {
        // Estado do aquecimento
        private static int? tempoRestante;
        private static int? potenciaAtual;
        private static string simboloAtual;
        private static bool pausado = false;
        private static bool emAquecimento = false;

        [HttpGet]
        public ActionResult<IEnumerable<object>> ListarProgramas()
        {
            var preDefinidos = ProgramasPreDefinidos.Lista.Select(p => new {
                p.Nome,
                p.Alimento,
                p.TempoSegundos,
                p.Potencia,
                p.StringPersonalizada,
                p.Instrucoes,
                Customizado = false
            });
            var customizados = ProgramasCustomizados.Lista.Select(p => new {
                p.Nome,
                p.Alimento,
                p.TempoSegundos,
                p.Potencia,
                p.StringPersonalizada,
                p.Instrucoes,
                Customizado = true
            });
            return Ok(preDefinidos.Concat(customizados));
        }

        [HttpPost("iniciar/{nome}")]
        public ActionResult<string> IniciarPrograma(string nome)
        {
            var programa = ProgramasPreDefinidos.Lista.FirstOrDefault(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
            if (programa == null)
                return NotFound("Programa não encontrado.");

            var service = new MicroondasService();
            var resultado = service.IniciarAquecimento(programa.TempoSegundos, programa.Potencia, programa.StringPersonalizada, true);
            tempoRestante = programa.TempoSegundos;
            potenciaAtual = programa.Potencia;
            simboloAtual = programa.StringPersonalizada;
            pausado = false;
            emAquecimento = true;
            return Ok(resultado);
        }

        [HttpPost("inicio-rapido")]
        public ActionResult<string> InicioRapido()
        {
            var service = new MicroondasService();
            var resultado = service.IniciarAquecimento();
            tempoRestante = 30;
            potenciaAtual = 10;
            simboloAtual = ".";
            pausado = false;
            emAquecimento = true;
            return Ok(resultado);
        }

        [HttpPost("pausar")]
        public ActionResult PausarOuCancelar()
        {
            if (!emAquecimento)
                return BadRequest("Não está em aquecimento.");
            if (pausado)
            {
                // Se já está pausado, cancela
                emAquecimento = false;
                pausado = false;
                tempoRestante = null;
                potenciaAtual = null;
                simboloAtual = null;
                return Ok("Aquecimento cancelado.");
            }
            pausado = true;

            return Ok("Aquecimento pausado.");
        }

        [HttpPost("continuar")]
        public ActionResult<string> Continuar()
        {
            if (!pausado || !emAquecimento || tempoRestante == null || potenciaAtual == null)
                return BadRequest("Não está pausado ou não há aquecimento para continuar.");
            var service = new MicroondasService();
            var resultado = service.IniciarAquecimento(tempoRestante, potenciaAtual, simboloAtual);
            pausado = false;
            return Ok(resultado);
        }

        [HttpPost("cadastrar")]
        public ActionResult CadastrarPrograma([FromBody] ProgramaCustomizado programa)
        {
            // Validação obrigatória
            if (string.IsNullOrWhiteSpace(programa.Nome) ||
                string.IsNullOrWhiteSpace(programa.Alimento) ||
                programa.Potencia < 1 || programa.Potencia > 10 ||
                string.IsNullOrWhiteSpace(programa.StringPersonalizada) ||
                programa.TempoSegundos < 1 || programa.TempoSegundos > 120)
            {
                return BadRequest("Todos os campos obrigatórios devem ser preenchidos corretamente.");
            }
            // Caractere não pode ser "." nem repetir
            var todosCaracteres = ProgramasPreDefinidos.Lista.Select(p => p.StringPersonalizada)
                .Concat(ProgramasCustomizados.Lista.Select(p => p.StringPersonalizada))
                .Append(".");
            if (todosCaracteres.Contains(programa.StringPersonalizada))
                return BadRequest("O caractere de aquecimento já está em uso ou é inválido.");

            ProgramasCustomizados.Adicionar(programa);
            return Ok("Programa customizado cadastrado com sucesso.");
        }
    }
}
