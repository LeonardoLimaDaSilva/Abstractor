using System;
using Abstractor.Cqrs.AzureStorage.Queue;

namespace Abstractor.Cqrs.AzureStorage.Interfaces
{
    /// <summary>
    ///     Defines the read operations of an Azure Queue repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface IQueueReader<out TEntity>
        where TEntity : AzureQueueMessage
    {
        /// <summary>
        ///     Gets the total number of messages into the queue.
        /// </summary>
        /// <returns>Number of messages.</returns>
        int Count();

        /// <summary>
        ///     Gets the next message from the queue.
        /// </summary>
        /// <param name="visibilityTimeout">Specifies the new visibility timeout of message.</param>
        /// <returns>Message.</returns>
        TEntity GetNext(TimeSpan? visibilityTimeout = null);
    }
}