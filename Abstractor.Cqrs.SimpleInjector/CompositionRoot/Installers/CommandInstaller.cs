using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.Operations;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    internal sealed class CommandInstaller : IAbstractorInstaller
    {
        public void RegisterServices(Container container, CompositionRootSettings settings)
        {
            if (settings.OperationAssemblies == null) return;

            container.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
          
            container.Register(typeof(ICommandHandler<>), settings.OperationAssemblies);

            container.Register<CommandPostAction>(Lifestyle.Scoped);
            container.Register<ICommandPostAction>(container.GetInstance<CommandPostAction>);

            container.RegisterDecorator(
               typeof(ICommandHandler<>),
               typeof(CommandTransactionDecorator<>)
            );

            container.RegisterDecorator(
                typeof(ICommandHandler<>),
                typeof(CommandPostActionDecorator<>)
           );

            container.RegisterDecorator(
                typeof(ICommandHandler<>),
                typeof(CommandEventDispatcherDecorator<>)
            );

            container.RegisterDecorator(
                typeof(ICommandHandler<>),
                typeof(CommandLifetimeScopeDecorator<>),
                Lifestyle.Singleton
            );
        }
    }
}
