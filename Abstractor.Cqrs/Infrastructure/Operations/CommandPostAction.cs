using System;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    /// <summary>
    ///     Registers an action event to be executed after a command.
    /// </summary>
    public sealed class CommandPostAction : ICommandPostAction
    {
        /// <summary>
        ///     Action event to be executed after a command.
        /// </summary>
        public event Action Execute = () => { };

        /// <summary>
        ///     Executes the action event registered in <see cref="ICommandPostAction.Execute" />.
        /// </summary>
        public void Act()
        {
            Execute();
        }

        /// <summary>
        ///     Resets the <see cref="ICommandPostAction.Execute" /> with an empty action.
        /// </summary>
        public void Reset()
        {
            Execute = () => { };
        }
    }
}