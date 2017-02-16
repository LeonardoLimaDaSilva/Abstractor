using System;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Exceptions
{
    /// <summary>
    ///     Exception thrown when a command handler is not found in the composition root.
    /// </summary>
    public class CommandHandlerNotFoundException : Exception
    {
        /// <summary>
        /// </summary>
        /// <param name="commandType"></param>
        public CommandHandlerNotFoundException(Type commandType)
            : base($"There is no handler registered to the command \"{commandType.FullName}\".")
        {
        }
    }
}