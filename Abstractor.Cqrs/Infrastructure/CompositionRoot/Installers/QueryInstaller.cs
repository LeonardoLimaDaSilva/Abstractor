using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    internal sealed class QueryInstaller : IShopDeliveryInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            Guard.ArgumentIsNotNull(settings.OperationAssemblies, nameof(settings.OperationAssemblies));

            container.RegisterSingleton<IQueryDispatcher, QueryDispatcher>();
            container.RegisterTransient(typeof (IQueryHandler<,>), settings.OperationAssemblies);
            container.RegisterTransient(typeof (IQueryAsyncHandler<,>), settings.OperationAssemblies);

            container.RegisterDecoratorSingleton(
                typeof (IQueryHandler<,>),
                typeof (QueryLifetimeScopeDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof (IQueryAsyncHandler<,>),
                typeof (QueryAsyncLifetimeScopeDecorator<,>));
        }
    }
}