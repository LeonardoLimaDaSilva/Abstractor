using System;
using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    /// Exceção da camada de aplicação utilizada para exibição de mensagens amigáveis na interface do usuário.
    /// </summary>
    public class OperationException : Exception
    {
        private readonly List<string> _errorMessages;

        public OperationException(string message)
            : base(message)
        {
            _errorMessages = new List<string>();
        }

        public OperationException(string message, Exception innerException)
            : base(message, innerException)
        {
            _errorMessages = new List<string>();
        }

        /// <summary>
        /// Adiciona uma mensagem de erro à exceção.
        /// </summary>
        /// <param name="message"></param>
        public OperationException AddErrorMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && !_errorMessages.Contains(message))
                _errorMessages.Add(message);

            return this;
        }

        /// <summary>
        /// Obtém uma lista com as mensagens de erro adicionais.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetErrorMessages()
        {
            return _errorMessages.Select(e => e);
        }
    }
}