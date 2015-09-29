using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    /// Um evento de comando que permite que o <see cref="IEvent"/> associado seja disparado após a execução do comando.
    /// </summary>
    public interface ICommandEvent : ICommand, IEvent
    {
        /// <summary>
        /// Determina se o evento deve ser disparado após a execução do comando.
        /// </summary>
        bool RaiseEvent();
    }
}
