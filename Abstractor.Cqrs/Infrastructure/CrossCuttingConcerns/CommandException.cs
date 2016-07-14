using System;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Base command exception that acts as an application event.
    /// </summary>
    public class CommandException : Exception, IApplicationEvent
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