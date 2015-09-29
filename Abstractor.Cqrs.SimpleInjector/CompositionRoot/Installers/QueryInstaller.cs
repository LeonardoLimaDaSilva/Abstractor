using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.Operations;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    internal sealed class QueryInstaller : IAbstractorInstaller
    {
        public void RegisterServices(Container container, CompositionRootSettings settings)
        {
            if (settings.OperationAssemblies == null) return;

            container.RegisterSingleton<IQueryDispatcher, QueryDispatcher>();

            container.Register(typeof(IQueryHandler<,>), settings.OperationAssemblies);
            container.Register(typeof(IQueryAsyncHandler<,>), settings.OperationAssemblies);

            container.RegisterDecorator(
                typeof(IQueryHandler<,>),
                typeof(QueryLifetimeScopeDecorator<,>),
                Lifestyle.Singleton
                );

            container.RegisterDecorator(
                typeof(IQueryHandler<,>),
                typeof(QueryNotNullDecorator<,>),
                Lifestyle.Singleton
                );

            container.RegisterDecorator(
                typeof(IQueryAsyncHandler<,>),
                typeof(QueryAsyncLifetimeScopeDecorator<,>),
                Lifestyle.Singleton
                );

            container.RegisterDecorator(
                typeof(IQueryAsyncHandler<,>),
                typeof(QueryAsyncNotNullDecorator<,>),
                Lifestyle.Singleton
                );
        }
    }
}
