using Abstractor.Cqrs.Infrastructure.Events;
using Abstractor.Cqrs.Interfaces.Events;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    internal sealed class EventInstaller : IAbstractorInstaller
    {
        public void RegisterServices(Container container, CompositionRootSettings settings)
        {
            if (settings.EventAssemblies == null) return;

            container.RegisterSingleton<IEventDispatcher, EventDispatcher>();

            container.Register(typeof(IEventHandler<>), settings.EventAssemblies);
           
            container.Register(typeof(IEventTrigger<>), typeof(MultipleDispatchEventTrigger<>));

            container.RegisterDecorator(
                typeof(IEventTrigger<>),
                typeof(TriggerEventWhenHandlersExistDecorator<>)
            );
        }
    }
}
