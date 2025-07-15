using System;
using MicroondasApp.Application;

namespace MicroondasApp
{
    public static class ExemploUsoMicroondas
    {
        public static void Executar()
        {
            var service = new MicroondasService();

            // Exemplo 1: tempo 5, potência 3
            Console.WriteLine(service.IniciarAquecimento(5, 3));

            // Exemplo 2: tempo 90, potência não informada
            Console.WriteLine(service.IniciarAquecimento(90, null));

            // Exemplo 3: início rápido (sem parâmetros)
            Console.WriteLine(service.IniciarAquecimento());
        }
    }
}
