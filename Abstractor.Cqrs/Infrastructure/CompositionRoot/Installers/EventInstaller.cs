using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Registers the services and decorators needed by the events operations.
    /// </summary>
    internal sealed class EventInstaller : IAbstractorInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            Guard.ArgumentIsNotNull(settings.OperationAssemblies, nameof(settings.OperationAssemblies));

            container.RegisterSingleton<IApplicationEventDispatcher, ApplicationEventDispatcher>();
            container.RegisterCollection(typeof (IApplicationEventHandler<>), settings.OperationAssemblies);

            container.RegisterSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
            container.RegisterCollection(typeof(IDomainEventHandler<>), settings.OperationAssemblies);

            container.RegisterDecoratorSingleton(
                typeof (IApplicationEventHandler<>),
                typeof (ApplicationEventLoggerDecorator<>));

            container.RegisterDecoratorSingleton(
                typeof(IDomainEventHandler<>),
                typeof(DomainEventLoggerDecorator<>));
        }
    }
}