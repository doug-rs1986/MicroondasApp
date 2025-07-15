using System;

namespace MicroondasApp.Domain
{
    /// <summary>
    /// Representa o tempo e a potência do aquecimento.
    /// </summary>
    public class Aquecimento
    {
        public int TempoSegundos { get; }
        public int Potencia { get; }

        public Aquecimento(int tempoSegundos, int? potencia)
        {
            if (tempoSegundos < 1 || tempoSegundos > 120)
                throw new RegrasDeNegocioException("O tempo deve estar entre 1 e 120 segundos.");

            if (potencia.HasValue && (potencia < 1 || potencia > 10))
                throw new RegrasDeNegocioException("A potência deve estar entre 1 e 10.");

            TempoSegundos = tempoSegundos;
            Potencia = potencia ?? 10;
        }

        /// <summary>
        /// Retorna o tempo formatado em MM:SS se aplicável.
        /// </summary>
        public string TempoFormatado()
        {
            if (TempoSegundos >= 60 && TempoSegundos < 120)
            {
                int minutos = TempoSegundos / 60;
                int segundos = TempoSegundos % 60;
                return $"{minutos}:{segundos:D2}";
            }
            return TempoSegundos.ToString();
        }
    }
}
