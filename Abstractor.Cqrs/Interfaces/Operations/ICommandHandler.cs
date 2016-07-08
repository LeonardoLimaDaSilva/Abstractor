namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Handler for the command that implements <see cref="ICommand" />.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        ///     Handles the <see cref="ICommand" />.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        void Handle(TCommand command);
    }
}