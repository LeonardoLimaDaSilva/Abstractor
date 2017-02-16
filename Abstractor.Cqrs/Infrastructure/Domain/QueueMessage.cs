namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Minimal representation of a queue message.
    /// </summary>
    public class QueueMessage
    {
        /// <summary>
        ///     The underlying message object.
        /// </summary>
        public object Object { get; set; }
    }
}