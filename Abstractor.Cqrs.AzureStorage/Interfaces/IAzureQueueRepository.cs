using System;
using Abstractor.Cqrs.AzureStorage.Queue;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    public interface IAzureQueueRepository<TEntity> where TEntity : AzureQueueMessage
    {
        void Add(TEntity entity);
        void Delete(TEntity entity);
        TEntity GetNext(TimeSpan? visibilityTimeout = null);
        int Count();
    }
}