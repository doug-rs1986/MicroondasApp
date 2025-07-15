using System;

namespace MicroondasApp.Domain
{
    /// <summary>
    /// Exceção específica para violações de regras de negócio do micro-ondas.
    /// </summary>
    public class RegrasDeNegocioException : Exception
    {
        public RegrasDeNegocioException(string mensagem) : base(mensagem) { }
    }
}
