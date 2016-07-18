using System;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Base command exception that can act as an application event or a command.
    /// </summary>
    public class CommandException : Exception, ICommand, IApplicationEvent
    {
        public CommandException()
        {
        }

        public CommandException(string message)
            : base(message)
        {
        }
    }
}