using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Base repository implementation specific for Azure Queue.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TAzureEntity">Azure entity type.</typeparam>
    public abstract class BaseQueueRepository<TEntity, TAzureEntity> : IQueueRepository<TEntity>
        where TEntity : QueueMessage
        where TAzureEntity : AzureQueueMessage
    {
        private readonly TimeSpan _visibilityTimeout;
        protected readonly IAzureQueueRepository<TAzureEntity> AzureQueueRepository;

        protected BaseQueueRepository(
            IAzureQueueRepository<TAzureEntity> azureQueueRepository,
            TimeSpan visibilityTimeout)
        {
            AzureQueueRepository = azureQueueRepository;
            _visibilityTimeout = visibilityTimeout;
        }

        /// <summary>
        ///     Adds a new message to the queue.
        /// </summary>
        /// <param name="message">Message to be added.</param>
        public void Add(TEntity message)
        {
            Guard.ArgumentIsNotNull(message, nameof(message));

            AzureQueueRepository.Add(ToAzureEntity(message));
        }

        /// <summary>
        ///     Removes a message from the queue.
        /// </summary>
        /// <param name="message">Message to be deleted.</param>
        public void Delete(TEntity message)
        {
            Guard.ArgumentIsNotNull(message, nameof(message));
            Guard.ArgumentIsNotNull(message.Object, nameof(message.Object));

            AzureQueueRepository.Delete((TAzureEntity) message.Object);
        }

        /// <summary>
        ///     Gets the next message from the queue using the visibility timeout passed in constructor.
        /// </summary>
        /// <returns>Message.</returns>
        public TEntity GetNext()
        {
            var message = AzureQueueRepository.GetNext(_visibilityTimeout);
            var entity = ToEntity(message);
            entity.Object = message;
            return entity;
        }

        /// <summary>
        ///     Gets the total number of messages into the queue.
        /// </summary>
        /// <returns>Number of messages.</returns>
        public int Count()
        {
            return AzureQueueRepository.Count();
        }

        /// <summary>
        ///     Hook to the actual mapping method.
        /// </summary>
        /// <param name="message">Azure message to be converted.</param>
        /// <returns>Converted entity message.</returns>
        public abstract TEntity ToEntity(TAzureEntity message);

        /// <summary>
        ///     Hook to the actual mapping method.
        /// </summary>
        /// <param name="message">Entity message to be converted</param>
        /// <returns>Converted Azure message.</returns>
        public abstract TAzureEntity ToAzureEntity(TEntity message);
    }
}