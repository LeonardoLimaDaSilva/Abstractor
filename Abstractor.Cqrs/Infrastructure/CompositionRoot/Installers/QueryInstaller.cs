using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Infrastructure.Operations.Dispatchers;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Registers the services and decorators needed by the queries operations.
    /// </summary>
    internal sealed class QueryInstaller : IAbstractorInstaller
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
                typeof(IQueryHandler<,>),
                typeof(QueryLoggerDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof (IQueryAsyncHandler<,>),
                typeof (QueryAsyncLifetimeScopeDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof(IQueryAsyncHandler<,>),
                typeof(QueryAsyncLoggerDecorator<,>));
        }
    }
}