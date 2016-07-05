using System;
using System.Threading.Tasks;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Processa comandos.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        ///     Executa o comando.
        /// </summary>
        /// <param name="command">Comando que será executado.</param>
        /// <exception cref="ArgumentNullException">Se o comando for nulo.</exception>
        void Dispatch(ICommand command);

        /// <summary>
        ///     Executa o comando de forma assíncrona.
        /// </summary>
        /// <param name="command">Comando que será executado.</param>
        /// <returns>Task para que o método seja awaitable.</returns>
        /// <exception cref="ArgumentNullException">Se o comando for nulo.</exception>
        Task DispatchAsync(ICommand command);
    }
}