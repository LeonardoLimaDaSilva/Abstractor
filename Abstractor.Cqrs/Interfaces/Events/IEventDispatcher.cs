namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Processa eventos.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        ///     Dispara o evento.
        /// </summary>
        /// <param name="event">Evento que será disparado.</param>
        void Dispatch(IEvent @event);
    }
}