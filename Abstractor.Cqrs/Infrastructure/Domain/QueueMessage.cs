namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Representação genérica de uma mensagem de fila.
    /// </summary>
    public class QueueMessage
    {
        public object DataMessage { get; set; }
    }
}