using System;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Domain;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
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

        public void Add(TEntity message)
        {
            Guard.ArgumentIsNotNull(message, nameof(message));

            AzureQueueRepository.Add(ToAzureEntity(message));
        }

        public void Delete(TEntity message)
        {
            Guard.ArgumentIsNotNull(message, nameof(message));
            Guard.ArgumentIsNotNull(message.DataMessage, nameof(message.DataMessage));

            AzureQueueRepository.Delete((TAzureEntity) message.DataMessage);
        }

        public TEntity GetNext()
        {
            var message = AzureQueueRepository.GetNext(_visibilityTimeout);
            var entity = ToEntity(message);
            entity.DataMessage = message;
            return entity;
        }

        public int Count()
        {
            return AzureQueueRepository.Count();
        }

        public abstract TEntity ToEntity(TAzureEntity message);
        public abstract TAzureEntity ToAzureEntity(TEntity message);
    }
}