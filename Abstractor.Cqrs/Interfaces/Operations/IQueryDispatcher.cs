using System;
using System.Threading.Tasks;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Dispatcher for a query handler.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        ///     Delegates the query parameters to the handler that implements <see cref="IQueryHandler{TQuery,TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <param name="query">Query to be dispatched.</param>
        /// <returns>Query result.</returns>
        TResult Dispatch<TResult>(IQuery<TResult> query);

        /// <summary>
        ///     Delegates the query parameters to the handler that implements <see cref="IQueryHandler{TQuery,TResult}" />.
        /// </summary>
        /// <typeparam name="TResult">Return type.</typeparam>
        /// <param name="query">Query to be dispatched.</param>
        /// <returns>Asynchronous task containing the query result.</returns>
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);
    }
}