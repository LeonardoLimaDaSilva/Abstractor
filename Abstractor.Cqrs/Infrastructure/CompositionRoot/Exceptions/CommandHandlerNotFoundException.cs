using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(Type commandType)
            : base($"There is no handler registered to the command \"{commandType.FullName}\".")
        {
        }
    }
}