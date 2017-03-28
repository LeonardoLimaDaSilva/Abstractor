using System;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Executes the action event registered in <see cref="ICommandPostAction.Execute" /> after the command is handled.
    /// </summary>
    /// <typeparam name="TCommand">Command to be executed.</typeparam>
    public sealed class CommandPostActionDecorator<TCommand> : CommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandPostAction _commandPostAction;
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;

        /// <summary>
        ///     CommandPostActionDecorator constructor.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="commandPostAction"></param>
        public CommandPostActionDecorator(
            Func<ICommandHandler<TCommand>> handler,
            ICommandPostAction commandPostAction)
        {
            _handlerFactory = handler;
            _commandPostAction = commandPostAction;
        }

        /// <summary>
        ///     Executes an action after the command is handled.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public override void Handle(TCommand command)
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