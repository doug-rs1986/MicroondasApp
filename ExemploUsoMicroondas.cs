using System;
using MicroondasApp.Application;

namespace MicroondasApp
{
    public static class ExemploUsoMicroondas
    {
        public static void Executar()
        {
            var service = new MicroondasService();

            // Exemplo 1: tempo 5, pot�ncia 3
            Console.WriteLine(service.IniciarAquecimento(5, 3));

            // Exemplo 2: tempo 90, pot�ncia n�o informada
            Console.WriteLine(service.IniciarAquecimento(90, null));

            // Exemplo 3: in�cio r�pido (sem par�metros)
            Console.WriteLine(service.IniciarAquecimento());
        }
    }
}
