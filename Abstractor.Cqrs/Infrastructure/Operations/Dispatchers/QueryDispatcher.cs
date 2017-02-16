using System;
using System.Linq;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for a query handler.
    /// </summary>
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IContainer _container;

        /// <summary>
        ///     QueryDispatcher constructor.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        public QueryDispatcher(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        ///     Delegates the query parameters to the handler that implements <see cref="IQueryHandler{TQuery,TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <param name="query">Query to be dispatched.</param>
        /// <returns>Query result.</returns>
        public TResult Dispatch<TResult>(IQuery<TResult> query)
        {
            Guard.ArgumentIsNotNull(query, nameof(query));

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = GetHandler(query, handlerType);

            return handler.Handle((dynamic) query);
        }

        /// <summary>
        ///     Delegates the query parameters to the handler that implements <see cref="IQueryHandler{TQuery,TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <param name="query">Query to be dispatched.</param>
        /// <returns>Asynchronous task containing the query result.</returns>
        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
        {
            Guard.ArgumentIsNotNull(query, nameof(query));

            var handlerType = typeof(IQueryAsyncHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = GetHandler(query, handlerType);

            return await handler.HandleAsync((dynamic) query);
        }

        /// <summary>
        ///     Gets a single instance of a query handler for the respective type or throws exceptions if none or multiple
        ///     instances are found.
        /// </summary>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <param name="query">Query to be dispatched.</param>
        /// <param name="handlerType">Generic handler type.</param>
        /// <returns></returns>
        private dynamic GetHandler<TResult>(IQuery<TResult> query, Type handlerType)
        {
            var instances = _container.GetAllInstances(handlerType).ToList();

            if (instances.Count > 1) throw new MultipleQueryHandlersException(query.GetType());

            dynamic handler = instances.SingleOrDefault();

            if (handler == null) throw new QueryHandlersNotFoundException(query.GetType());

            return handler;
        }
    }
}