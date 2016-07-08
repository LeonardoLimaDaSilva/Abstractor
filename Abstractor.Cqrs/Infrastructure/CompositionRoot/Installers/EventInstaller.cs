using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    internal sealed class EventInstaller : IAbstractorInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            Guard.ArgumentIsNotNull(settings.EventAssemblies, nameof(settings.EventAssemblies));

            container.RegisterSingleton<IEventDispatcher, EventDispatcher>();
            container.RegisterCollection(typeof (IEventHandler<>), settings.EventAssemblies);

            container.RegisterDecoratorSingleton(
                typeof(IEventHandler<>),
                typeof(EventLoggerDecorator<>));
        }
    }
}