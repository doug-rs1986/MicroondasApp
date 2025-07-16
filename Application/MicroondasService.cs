using System.Text.Json;
using System.Text.Json.Serialization;
using MicroondasApp.Domain;

namespace MicroondasApp.Application
{
    /// <summary>
    /// Serviço para simular o funcionamento do microondas.
    /// </summary>
    public class MicroondasService
    {
        /// <summary>
        /// Inicia o aquecimento com tempo e potência informados ou modo rápido.
        /// </summary>
        public string IniciarAquecimento(int? tempoSegundos = null, int? potencia = null, string stringPersonalizada = null, bool programaPreDefinido = false)
        {
            // Modo início rápido
            if (!tempoSegundos.HasValue && !potencia.HasValue)
            {
                tempoSegundos = 30;
                potencia = 10;
            }

            if (!programaPreDefinido)
            {
            // Se for programa pré-definido, não valida tempo
                if (tempoSegundos < 1 || tempoSegundos > 120)
                    throw new RegrasDeNegocioException("O tempo deve estar entre 1 e 120 segundos.");
            }

            var aquecimento = new Aquecimento(tempoSegundos ?? 30, potencia);
            string simbolo = stringPersonalizada ?? ".";
            string pontos = string.Join(" ", Enumerable.Repeat(new string(simbolo[0], aquecimento.Potencia), aquecimento.TempoSegundos));
            string tempoFormatado = aquecimento.TempoFormatado();
            return $"{pontos} Aquecimento concluído (Tempo: {tempoFormatado}, Potência: {aquecimento.Potencia})";
        }
    }

    public class ProgramaPreDefinido
    {
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public int TempoSegundos { get; set; }
        public int Potencia { get; set; }
        public string StringPersonalizada { get; set; }
        public string Instrucoes { get; set; }

        public virtual bool Customizado => false;
    }

    public class ProgramaCustomizado : ProgramaPreDefinido
    {
        [JsonIgnore]
        public override bool Customizado => true;
    }

    public static class ProgramasPreDefinidos
    {
        public static List<ProgramaPreDefinido> Lista { get; } = new()
        {
            new ProgramaPreDefinido {
                Nome = "Pipoca",
                Alimento = "Pipoca (de microondas)",
                TempoSegundos = 180, // 3 minutos
                Potencia = 7,
                StringPersonalizada = "*",
                Instrucoes = "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."
            },
            new ProgramaPreDefinido {
                Nome = "Leite",
                Alimento = "Leite",
                TempoSegundos = 300, // 5 minutos
                Potencia = 5,
                StringPersonalizada = "#",
                Instrucoes = "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras."
            },
            new ProgramaPreDefinido {
                Nome = "Carnes de boi",
                Alimento = "Carne em pedaço ou fatias",
                TempoSegundos = 840, // 14 minutos
                Potencia = 4,
                StringPersonalizada = "~",
                Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."
            },
            new ProgramaPreDefinido {
                Nome = "Frango",
                Alimento = "Frango (qualquer corte)",
                TempoSegundos = 480, // 8 minutos
                Potencia = 7,
                StringPersonalizada = "@",
                Instrucoes = "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."
            },
            new ProgramaPreDefinido {
                Nome = "Feijão",
                Alimento = "Feijão congelado",
                TempoSegundos = 480, // 8 minutos
                Potencia = 9,
                StringPersonalizada = "+",
                Instrucoes = "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas."
            }
        };
    }

    public static class ProgramasCustomizados
    {
        private static readonly string CaminhoArquivo = "programas_customizados.json";
        private static List<ProgramaCustomizado> lista;

        public static List<ProgramaCustomizado> Lista
        {
            get
            {
                if (lista == null)
                    Carregar();
                return lista;
            }
        }

        public static void Carregar()
        {
            if (File.Exists(CaminhoArquivo))
            {
                var json = File.ReadAllText(CaminhoArquivo);
                lista = JsonSerializer.Deserialize<List<ProgramaCustomizado>>(json) ?? new List<ProgramaCustomizado>();
            }
            else
            {
                lista = new List<ProgramaCustomizado>();
            }
        }

        public static void Salvar()
        {
            var json = JsonSerializer.Serialize(lista);
            File.WriteAllText(CaminhoArquivo, json);
        }

        public static void Adicionar(ProgramaCustomizado programa)
        {
            lista.Add(programa);
            Salvar();
        }
    }
}
