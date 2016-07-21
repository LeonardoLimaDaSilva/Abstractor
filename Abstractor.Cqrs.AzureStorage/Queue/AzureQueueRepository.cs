using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Repository implementation specific for Azure Queue.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal sealed class AzureQueueRepository<TEntity> : IAzureQueueRepository<TEntity>
        where TEntity : AzureQueueMessage, new()
    {
        private readonly AzureQueueContext _context;

        public AzureQueueRepository(IAzureQueueContext context)
        {
            _context = (AzureQueueContext)context;
        }

        /// <summary>
        ///     Adds a new message to the queue.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        public void Add(TEntity entity)
        {
            var set = _context.Set<TEntity>();
            set.Insert(entity);
        }

        /// <summary>
        ///     Removes a message from the queue.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void Delete(TEntity entity)
        {
            var set = _context.Set<TEntity>();
            set.Delete(entity);
        }

        /// <summary>
        ///     Gets the next message from the queue.
        /// </summary>
        /// <param name="visibilityTimeout">Specifies the new visibility timeout of message.</param>
        /// <returns>Message.</returns>
        public TEntity GetNext(TimeSpan? visibilityTimeout = null)
        {
            var set = (AzureQueueSet<TEntity>) _context.Set<TEntity>();
            return set.GetNext(visibilityTimeout);
        }

        /// <summary>
        ///     Gets the total number of messages into the queue.
        /// </summary>
        /// <returns>Number of messages.</returns>
        public int Count()
        {
            var set = (AzureQueueSet<TEntity>) _context.Set<TEntity>();
            return set.Count();
        }
    }
}