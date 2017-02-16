using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    /// <summary>
    ///     Exception thown when multiple classes are registered to handle a query type.
    /// </summary>
    public class MultipleQueryHandlersException : Exception
    {
        /// <summary>
        /// </summary>
        /// <param name="queryType"></param>
        public MultipleQueryHandlersException(Type queryType)
            : base($"Multiple classes are registered to handle the query \"{queryType.FullName}\".")
        {
        }
    }
}