using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Defines a generic queue message repository.
    /// </summary>
    /// <typeparam name="TEntity">Message tipe.</typeparam>
    public interface IQueueRepository<TEntity> where TEntity : QueueMessage
    {
        void Add(TEntity message);
        void Delete(TEntity message);
        TEntity GetNext();
        int Count();
    }
}