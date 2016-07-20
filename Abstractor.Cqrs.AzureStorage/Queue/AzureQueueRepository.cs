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
        private readonly Func<AzureQueueContext> _contextProvider;

        public AzureQueueRepository(Func<AzureQueueContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        /// <summary>
        ///     Adds a new message to the queue.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        public void Add(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Insert(entity);
        }

        /// <summary>
        ///     Removes a message from the queue.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        public void Delete(TEntity entity)
        {
            var tbset = _contextProvider().Set<TEntity>();
            tbset.Delete(entity);
        }

        /// <summary>
        ///     Gets the next message from the queue.
        /// </summary>
        /// <param name="visibilityTimeout">Specifies the new visibility timeout of message.</param>
        /// <returns>Message.</returns>
        public TEntity GetNext(TimeSpan? visibilityTimeout = null)
        {
            var tbset = (AzureQueueSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.GetNext(visibilityTimeout);
        }

        /// <summary>
        ///     Gets the total number of messages into the queue.
        /// </summary>
        /// <returns>Number of messages.</returns>
        public int Count()
        {
            var tbset = (AzureQueueSet<TEntity>) _contextProvider().Set<TEntity>();
            return tbset.Count();
        }
    }
}