using System;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Base command exception that acts as an event listener.
    /// </summary>
    public class CommandException : Exception, IEventListener
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