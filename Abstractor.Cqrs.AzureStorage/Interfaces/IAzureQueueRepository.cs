using Abstractor.Cqrs.AzureStorage.Queue;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Represents a repository of an Azure Queue.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IAzureQueueRepository<TEntity> : IQueueWriter<TEntity>, IQueueReader<TEntity> 
        where TEntity : AzureQueueMessage
    {
    }
}