using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Events;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    internal sealed class EventInstaller : IShopDeliveryInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            Guard.ArgumentIsNotNull(settings.EventAssemblies, nameof(settings.EventAssemblies));

            container.RegisterSingleton<IEventDispatcher, EventDispatcher>();
            container.RegisterTransient(typeof (IEventHandler<>), settings.EventAssemblies);
            container.RegisterTransient(typeof (IEventTrigger<>), typeof (MultipleDispatchEventTrigger<>));

            container.RegisterDecoratorTransient(
                typeof (IEventTrigger<>),
                typeof (TriggerRegisteredEventsDecorator<>));
        }
    }
}