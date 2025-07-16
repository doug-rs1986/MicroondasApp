using Microsoft.AspNetCore.Mvc;
using MicroondasApp.Application;

namespace MicroondasApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MicroondasController : ControllerBase
    {
        private readonly MicroondasService _microondasService;
        private readonly MicroondasStateService _stateService;

        public MicroondasController(MicroondasService microondasService, MicroondasStateService stateService)
        {
            _microondasService = microondasService;
            _stateService = stateService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> ListarProgramas()
        {
            var preDefinidos = ProgramasPreDefinidos.Lista.Select((p, idx) => new {
                Id = idx + 1,
                p.Nome,
                p.Alimento,
                p.TempoSegundos,
                p.Potencia,
                p.StringPersonalizada,
                p.Instrucoes,
                Customizado = false
            });
            int offset = ProgramasPreDefinidos.Lista.Count;
            var customizados = ProgramasCustomizados.Lista.Select((p, idx) => new {
                Id = offset + idx + 1,
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

        [HttpPost("iniciar")]
        public ActionResult<string> Iniciar([FromBody] IniciarRequest request)
        {
            if (request == null)
                return BadRequest("Requisi��o inv�lida.");

            ProgramaPreDefinido? programa = null;
            var totalPreDefinidos = ProgramasPreDefinidos.Lista.Count;
            var totalCustomizados = ProgramasCustomizados.Lista.Count;

            if (request.Id.HasValue)
            {
                int id = request.Id.Value;
                if (id >= 1 && id <= totalPreDefinidos)
                {
                    programa = ProgramasPreDefinidos.Lista[id - 1];
                }
                else if (id > totalPreDefinidos && id <= totalPreDefinidos + totalCustomizados)
                {
                    programa = ProgramasCustomizados.Lista[id - totalPreDefinidos - 1];
                }
                if (programa == null)
                    return NotFound("Programa n�o encontrado.");
                // N�o permite alterar tempo/pot�ncia de programas
                var resultado = _microondasService.IniciarAquecimento(programa.TempoSegundos, programa.Potencia, programa.StringPersonalizada, true);
                _stateService.TempoRestante = programa.TempoSegundos;
                _stateService.PotenciaAtual = programa.Potencia;
                _stateService.SimboloAtual = programa.StringPersonalizada;
                _stateService.Pausado = false;
                _stateService.EmAquecimento = true;
                return Ok(resultado);
            }

            // Se s� pot�ncia for informada, retorna erro amig�vel
            if ((request.TempoSegundos <= 0 || request.TempoSegundos == 0) && request.Potencia > 0)
                throw new MicroondasApp.Domain.RegrasDeNegocioException("Para definir a pot�ncia, � necess�rio informar tamb�m o tempo de aquecimento.");

            // Se n�o tem Id, segue l�gica manual
            int tempo = 30;
            int potencia = 10;
            if (request.TempoSegundos > 0 && request.Potencia > 0)
            {
                tempo = request.TempoSegundos;
                potencia = request.Potencia;
            }
            else if (request.TempoSegundos > 0)
            {
                tempo = request.TempoSegundos;
            }
            // Valida��o
            if (tempo < 1 || tempo > 120 || potencia < 1 || potencia > 10)
                return BadRequest("Tempo deve estar entre 1 e 120 segundos e pot�ncia entre 1 e 10.");
            var simbolo = ".";
            var resultadoManual = _microondasService.IniciarAquecimento(tempo, potencia, simbolo, false);
            _stateService.TempoRestante = tempo;
            _stateService.PotenciaAtual = potencia;
            _stateService.SimboloAtual = simbolo;
            _stateService.Pausado = false;
            _stateService.EmAquecimento = true;
            return Ok(resultadoManual);
        }

        [HttpPost("pausar")]
        public ActionResult PausarOuCancelar()
        {
            if (!_stateService.EmAquecimento)
                return BadRequest("N�o est� em aquecimento.");
            if (_stateService.Pausado)
            {
                // Se j� est� pausado, cancela
                _stateService.Reset();
                return Ok("Aquecimento cancelado.");
            }
            _stateService.Pausado = true;
            return Ok("Aquecimento pausado.");
        }

        [HttpPost("continuar")]
        public ActionResult<string> Continuar()
        {
            if (!_stateService.Pausado || !_stateService.EmAquecimento || _stateService.TempoRestante == null || _stateService.PotenciaAtual == null)
                return BadRequest("N�o est� pausado ou n�o h� aquecimento para continuar.");
            var resultado = _microondasService.IniciarAquecimento(_stateService.TempoRestante, _stateService.PotenciaAtual, _stateService.SimboloAtual);
            _stateService.Pausado = false;
            return Ok(resultado);
        }

        [HttpPost("cadastrar")]
        public ActionResult CadastrarPrograma([FromBody] ProgramaCustomizado programa)
        {
            // Valida��o obrigat�ria
            if (string.IsNullOrWhiteSpace(programa.Nome) ||
                string.IsNullOrWhiteSpace(programa.Alimento) ||
                programa.Potencia < 1 || programa.Potencia > 10 ||
                string.IsNullOrWhiteSpace(programa.StringPersonalizada) ||
                programa.TempoSegundos < 1 || programa.TempoSegundos > 120)
            {
                return BadRequest("Todos os campos obrigat�rios devem ser preenchidos corretamente.");
            }
            // Caractere n�o pode ser "." nem repetir
            var todosCaracteres = ProgramasPreDefinidos.Lista.Select(p => p.StringPersonalizada)
                .Concat(ProgramasCustomizados.Lista.Select(p => p.StringPersonalizada))
                .Append(".");
            if (todosCaracteres.Contains(programa.StringPersonalizada))
                return BadRequest("O caractere de aquecimento j� est� em uso ou � inv�lido.");

            ProgramasCustomizados.Adicionar(programa);
            return Ok("Programa customizado cadastrado com sucesso.");
        }

        public class IniciarRequest
        {
            public int? Id { get; set; }
            public int TempoSegundos { get; set; }
            public int Potencia { get; set; }
        }
    }
}
