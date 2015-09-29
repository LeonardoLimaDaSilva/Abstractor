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

        public void Dispatch(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            handler.Handle((dynamic)command);
        }

        public async Task DispatchAsync(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);

            await Task.Run(() =>
            {
                handler.Handle((dynamic)command);
            });
        }
    }
}