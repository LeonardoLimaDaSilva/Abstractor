using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Executes the action event registered in <see cref="ICommandPostAction.Execute" /> after the command is handled.
    /// </summary>
    /// <typeparam name="TCommand">Command to be executed.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandPostActionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandPostAction _commandPostAction;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        public CommandPostActionDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory,
            ICommandPostAction commandPostAction)
        {
            _handlerFactory = handlerFactory;
            _commandPostAction = commandPostAction;
        }

        /// <summary>
        ///     Executes an action after the command is handled.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public void Handle(TCommand command)
        {
            try
            {
                _handlerFactory().Handle(command);
                _commandPostAction.Act();
            }
            finally
            {
                _commandPostAction.Reset();
            }
        }
    }
}