using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Registers the services and decorators needed by the commands operations.
    /// </summary>
    internal sealed class CommandInstaller : IAbstractorInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            Guard.ArgumentIsNotNull(settings.ApplicationAssemblies, nameof(settings.ApplicationAssemblies));

            container.RegisterSingleton<ICommandDispatcher, CommandDispatcher>();
            container.RegisterTransient(typeof (ICommandHandler<>), settings.ApplicationAssemblies);
            container.RegisterScoped<ICommandPostAction, CommandPostAction>();

            container.RegisterDecoratorTransient(
                typeof (ICommandHandler<>),
                typeof (DomainEventDispatcherDecorator<>));

            container.RegisterDecoratorTransient(
                typeof (ICommandHandler<>),
                typeof (CommandTransactionDecorator<>));

            container.RegisterDecoratorTransient(
                typeof (ICommandHandler<>),
                typeof (CommandPostActionDecorator<>));

            container.RegisterDecoratorSingleton(
                typeof (ICommandHandler<>),
                typeof (ApplicationEventDispatcherDecorator<>));

            container.RegisterDecoratorSingleton(
                typeof (ICommandHandler<>),
                typeof (CommandValidationDecorator<>));

            container.RegisterDecoratorSingleton(
                typeof (ICommandHandler<>),
                typeof (CommandLoggerDecorator<>));

            container.RegisterDecoratorSingleton(
                typeof(ICommandHandler<>),
                typeof(CommandLifetimeScopeDecorator<>));
        }
    }
}