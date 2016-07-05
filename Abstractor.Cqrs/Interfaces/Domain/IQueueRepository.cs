using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Define um repositório genérico de mensagens de fila.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da mensagem.</typeparam>
    public interface IQueueRepository<TEntity> where TEntity : QueueMessage
    {
        void Add(TEntity message);
        void Delete(TEntity message);
        TEntity GetNext();
        int Count();
    }
}