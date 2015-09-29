using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.SimpleInjector.CompositionRoot.Adapters;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    internal sealed class ContainerInstaller : IAbstractorInstaller
    {
        public void RegisterServices(Container container, CompositionRootSettings settings)
        {
            if (settings.OperationAssemblies == null) return;

            container.RegisterSingleton<IContainer, ContainerAdapter>();
        }
    }
}