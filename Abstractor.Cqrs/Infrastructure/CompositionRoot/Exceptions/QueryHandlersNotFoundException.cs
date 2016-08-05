using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    public class QueryHandlersNotFoundException : Exception
    {
        public QueryHandlersNotFoundException(Type queryType)
            : base($"There are no handlers registered to the query \"{queryType.FullName}\".")
        {
        }
    }
}