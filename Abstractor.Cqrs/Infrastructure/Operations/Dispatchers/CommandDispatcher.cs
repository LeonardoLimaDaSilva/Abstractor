using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Processador de comandos.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public CommandDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Dispara um <see cref="ICommandHandler{ICommand}" /> registrado em <see cref="IContainer" />.
        /// </summary>
        /// <param name="command">Objeto de comando.</param>
        public void Dispatch(ICommand command)
        {
            Guard.ArgumentIsNotNull(command, nameof(command));

            var handlerType = typeof (ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            handler.Handle((dynamic) command);
        }

        /// <summary>
        ///     Dispara, de forma assíncrona, um <see cref="ICommandHandler{ICommand}" /> registrado em <see cref="IContainer" />.
        /// </summary>
        /// <param name="command">Objeto de comando.</param>
        /// <returns>Task assíncrona.</returns>
        public async Task DispatchAsync(ICommand command)
        {
            Guard.ArgumentIsNotNull(command, nameof(command));

            var handlerType = typeof (ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            await Task.Run(() => { handler.Handle((dynamic) command); });
        }
    }
}