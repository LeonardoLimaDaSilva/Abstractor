using System;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Ensures that there is a lifetime scope before the query execution.
    /// </summary>
    /// <typeparam name="TQuery">Query to be handled.</typeparam>
    /// <typeparam name="TResult">Return type.</typeparam>
    public sealed class QueryLifetimeScopeDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IContainer _container;
        private readonly Func<IQueryHandler<TQuery, TResult>> _handlerFactory;

        /// <summary>
        ///     QueryLifetimeScopeDecorator constructor.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="handlerFactory"></param>
        public QueryLifetimeScopeDecorator(
            IContainer container,
            Func<IQueryHandler<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        /// <summary>
        ///     Ensures that there is a lifetime scope before the query execution.
        /// </summary>
        /// <param name="query">Query to be handled.</param>
        /// <returns>Query result.</returns>
        public TResult Handle(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(query);

            using (_container.BeginLifetimeScope())
            {
                return _handlerFactory().Handle(query);
            }
        }
    }
}