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

            container.RegisterSingleton<IEventDispatcher, EventDispatcher>();
            container.RegisterCollection(typeof (IEventHandler<>), settings.OperationAssemblies);

            container.RegisterDecoratorSingleton(
                typeof(IEventHandler<>),
                typeof(EventLoggerDecorator<>));
        }
    }
}