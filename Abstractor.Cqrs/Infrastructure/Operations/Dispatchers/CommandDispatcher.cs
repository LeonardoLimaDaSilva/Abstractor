using System;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for a command handler.
    /// </summary>
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        /// <summary>
        /// CommandDispatcher constructor.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        public CommandDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Delegates the command parameters to the handler that implements <see cref="ICommandHandler{ICommand}" />.
        /// </summary>
        /// <param name="command">Command to be dispatched.</param>
        public void Dispatch(ICommand command)
        {
            Guard.ArgumentIsNotNull(command, nameof(command));

            try
            {
                GetCommandHandler(command.GetType()).Handle((dynamic) command);
            }
            catch (CommandException ex)
            {
                GetCommandHandler(ex.GetType()).Handle((dynamic) ex);
                throw;
            }
        }

        /// <summary>
        ///     Delegates the command parameters asynchronously to the handler that implements
        ///     <see cref="ICommandHandler{ICommand}" />.
        /// </summary>
        /// <param name="command">Command to be dispatched.</param>
        /// <returns>Asynchronous task.</returns>
        public async Task DispatchAsync(ICommand command)
        {
            Guard.ArgumentIsNotNull(command, nameof(command));

            await Task.Run(() =>
            {
                try
                {
                    GetCommandHandler(command.GetType()).Handle((dynamic)command); 
                }
                catch (CommandException ex)
                {
                    GetCommandHandler(ex.GetType()).Handle((dynamic)ex);
                    throw;
                }
            });
        }

        private dynamic GetCommandHandler(Type commandType)
        {
            var handlerType = typeof (ICommandHandler<>).MakeGenericType(commandType);

            try
            {
                return _container.GetInstance(handlerType);
            }
            catch (Exception ex)
            {
                if (_container.IsActivationException(ex))
                    throw new CommandHandlerNotFoundException(commandType);

                throw;
            }
        }
    }
}