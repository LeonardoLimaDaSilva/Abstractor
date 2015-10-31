using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    public sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IContainer _container;

        public CommandDispatcher(IContainer container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public void Dispatch(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            handler.Handle((dynamic)command);
        }

        [DebuggerStepThrough]
        public async Task DispatchAsync(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            await Task.Run(() =>
            {
                handler.Handle((dynamic)command);
            });
        }
    }
}