using Abstractor.Cqrs.Infrastructure.Domain;

namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Defines a generic queue message repository.
    /// </summary>
    /// <typeparam name="TEntity">Message tipe.</typeparam>
    public interface IQueueRepository<TEntity> where TEntity : QueueMessage
    {
        /// <summary>
        ///     Adds the message to the underlying queue implementation.
        /// </summary>
        /// <param name="message">Message entity.</param>
        void Add(TEntity message);

        /// <summary>
        ///     Returns the total count of enqueued messages of the current type.
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        ///     Deletes the message from the underlying queue implementation.
        /// </summary>
        /// <param name="message">Message entity.</param>
        void Delete(TEntity message);

        /// <summary>
        ///     Gets the next message from the underlying queue implementation.
        /// </summary>
        TEntity GetNext();
    }
}