namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Representa��o gen�rica de uma mensagem de fila.
    /// </summary>
    public class QueueMessage
    {
        public object DataMessage { get; set; }
    }
}