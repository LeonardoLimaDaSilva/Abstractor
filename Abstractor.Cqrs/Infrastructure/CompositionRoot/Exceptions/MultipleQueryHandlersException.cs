using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    public class MultipleQueryHandlersException : Exception
    {
        public MultipleQueryHandlersException(Type queryType) 
            : base($"Multiple classes are registered to handle the query \"{queryType.FullName}\".")
        {
        }
    }
}