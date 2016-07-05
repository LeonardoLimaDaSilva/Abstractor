namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Manipula o <typeparamref name="TCommand" />.
    /// </summary>
    /// <typeparam name="TCommand">Comando que será manipulado.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        ///     Manipula o <typeparamref name="TCommand" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Uma tarefa assíncrona que pode ser aguardada (await).</returns>
        void Handle(TCommand command);
    }
}