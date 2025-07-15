using System;

namespace MicroondasApp.Domain
{
    /// <summary>
    /// Exce��o espec�fica para viola��es de regras de neg�cio do micro-ondas.
    /// </summary>
    public class RegrasDeNegocioException : Exception
    {
        public RegrasDeNegocioException(string mensagem) : base(mensagem) { }
    }
}
