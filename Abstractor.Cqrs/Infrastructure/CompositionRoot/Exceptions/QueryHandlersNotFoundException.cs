using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    /// <summary>
    ///     Exception thrown when a query handler is not found in the composition root.
    /// </summary>
    public class QueryHandlersNotFoundException : Exception
    {
        /// <summary>
        /// </summary>
        /// <param name="queryType"></param>
        public QueryHandlersNotFoundException(Type queryType)
            : base($"There are no handlers registered to the query \"{queryType.FullName}\".")
        {
        }
    }
}