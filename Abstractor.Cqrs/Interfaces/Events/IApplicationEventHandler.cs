namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Event handler that subscribes to the <see cref="IApplicationEvent" />.
    /// </summary>
    /// <typeparam name="TEvent">Application event in which the handler subscribes to.</typeparam>
    public interface IApplicationEventHandler<in TEvent> where TEvent : IApplicationEvent
    {
        /// <summary>
        ///     Handles the <see cref="IApplicationEvent" />.
        /// </summary>
        /// <param name="applicationEvent">Application event to be handled.</param>
        void Handle(TEvent applicationEvent);
    }
}