using System;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Registers an action event to be executed after a command.
    /// </summary>
    public interface ICommandPostAction
    {
        /// <summary>
        ///     Executes the action event registered in <see cref="ICommandPostAction.Execute" />.
        /// </summary>
        void Act();

        /// <summary>
        ///     Action event to be executed after a command.
        /// </summary>
        event Action Execute;

        /// <summary>
        ///     Resets the <see cref="ICommandPostAction.Execute" /> with an empty action.
        /// </summary>
        void Reset();
    }
}