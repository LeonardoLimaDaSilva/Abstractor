﻿using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
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
            Guard.ArgumentIsNotNull(settings.ApplicationAssemblies, nameof(settings.ApplicationAssemblies));

            container.RegisterSingleton<IQueryDispatcher, QueryDispatcher>();

            container.RegisterCollection(typeof(IQueryHandler<,>), settings.ApplicationAssemblies);
            container.RegisterCollection(typeof(IQueryAsyncHandler<,>), settings.ApplicationAssemblies);

            container.RegisterDecoratorSingleton(
                typeof(IQueryHandler<,>),
                typeof(QueryLoggerDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof(IQueryHandler<,>),
                typeof(QueryLifetimeScopeDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof(IQueryAsyncHandler<,>),
                typeof(QueryAsyncLoggerDecorator<,>));

            container.RegisterDecoratorSingleton(
                typeof(IQueryAsyncHandler<,>),
                typeof(QueryAsyncLifetimeScopeDecorator<,>));
        }
    }
}