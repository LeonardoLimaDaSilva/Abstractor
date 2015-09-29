using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    public sealed class QueryAsyncLifetimeScopeDecorator<TQuery, TResult> : IQueryAsyncHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IContainer _container;
        private readonly Func<IQueryAsyncHandler<TQuery, TResult>> _handlerFactory;

        public QueryAsyncLifetimeScopeDecorator(
            IContainer container,
            Func<IQueryAsyncHandler<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public Task<TResult> HandleAsync(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().HandleAsync(query);
            using (_container.BeginLifetimeScope())
                return _handlerFactory().HandleAsync(query);
        }
    }
}