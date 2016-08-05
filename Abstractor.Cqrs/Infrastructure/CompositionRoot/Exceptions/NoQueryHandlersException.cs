using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    public class NoQueryHandlersException : Exception
    {
        public NoQueryHandlersException(Type queryType)
            : base($"There are no classes registered to handle the query \"{queryType.FullName}\".")
        {
        }
    }
}