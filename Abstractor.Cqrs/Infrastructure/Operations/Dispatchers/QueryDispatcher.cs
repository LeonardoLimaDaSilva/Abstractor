using System.Diagnostics;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Dispatchers
{
    /// <summary>
    ///     Dispatcher for a query handler.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly IContainer _container;

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

            var handlerType = typeof (IQueryHandler<,>).MakeGenericType(query.GetType(), typeof (TResult));
            dynamic handler = _container.GetInstance(handlerType);

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

            var handlerType = typeof (IQueryAsyncHandler<,>).MakeGenericType(query.GetType(), typeof (TResult));
            dynamic handler = _container.GetInstance(handlerType);

            return await handler.HandleAsync((dynamic) query);
        }
    }
}