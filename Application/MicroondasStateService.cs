namespace MicroondasApp.Application
{
    public class MicroondasStateService
    {
        public int? TempoRestante { get; set; }
        public int? PotenciaAtual { get; set; }
        public string? SimboloAtual { get; set; }
        public bool Pausado { get; set; } = false;
        public bool EmAquecimento { get; set; } = false;

        public void Reset()
        {
            TempoRestante = null;
            PotenciaAtual = null;
            SimboloAtual = null;
            Pausado = false;
            EmAquecimento = false;
        }
    }
}
