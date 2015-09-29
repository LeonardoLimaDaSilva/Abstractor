using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    public sealed class QueryLifetimeScopeDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IContainer _container;
        private readonly Func<IQueryHandler<TQuery, TResult>> _handlerFactory;

        public QueryLifetimeScopeDecorator(
            IContainer container,
            Func<IQueryHandler<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(query);
            using (_container.BeginLifetimeScope())
                return _handlerFactory().Handle(query);
        }
    }
}
