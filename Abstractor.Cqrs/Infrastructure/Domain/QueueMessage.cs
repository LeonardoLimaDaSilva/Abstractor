namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Minimal representation of a queue message.
    /// </summary>
    public class QueueMessage
    {
        public object DataMessage { get; set; }
    }
}