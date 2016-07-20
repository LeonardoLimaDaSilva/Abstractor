using Abstractor.Cqrs.AzureStorage.Queue;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Defines the write operations of an Azure Queue repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IQueueWriter<in TEntity>
        where TEntity : AzureQueueMessage
    {
        /// <summary>
        ///     Adds a new message to the queue.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        void Add(TEntity entity);

        /// <summary>
        ///     Removes a message from the queue.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        void Delete(TEntity entity);
    }
}