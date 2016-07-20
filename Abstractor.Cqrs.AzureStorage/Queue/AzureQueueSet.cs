using System;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Azure Queue specific implementation of a data set.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public sealed class AzureQueueSet<TEntity> : BaseDataSet<TEntity>
        where TEntity : AzureQueueMessage, new()
    {
        private readonly CloudQueue _queue;

        public AzureQueueSet(string connectionString) : base(false)
        {
            var queueClient = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient();
            var queueName = typeof (TEntity).GetQueueName();
            _queue = queueClient.GetQueueReference(queueName);
            _queue.CreateIfNotExists();
        }

        /// <summary>
        ///     Adds the message to the queue.
        /// </summary>
        /// <param name="entity">Entity to be inserted.</param>
        protected override void InsertEntity(TEntity entity)
        {
            _queue.AddMessage(ToCloudQueueMessage(entity));
        }

        /// <summary>
        ///     Removes the message from the queue.
        /// </summary>
        /// <param name="entity">Entity to be removed.</param>
        protected override void DeleteEntity(TEntity entity)
        {
            _queue.DeleteMessage(ToCloudQueueMessage(entity));
        }

        /// <summary>
        ///     Updates an existing message into the queue. Caution, this method is not implemented yet.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        protected override void UpdateEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets a message from the queue using the default request options.
        /// </summary>
        /// <param name="entity">Entity definition.</param>
        /// <returns>Message queue.</returns>
        protected override TEntity Get(TEntity entity)
        {
            var message = _queue.GetMessage();
            return ToEntity(message);
        }

        /// <summary>
        ///     Gets a message from the queue using the default request options, and marks the retrieved message as invisible in
        ///     the queue for the visibility timeout period.
        /// </summary>
        /// <param name="visibilityTimeout">Period that the message will be invisible in the queue.</param>
        /// <returns>Message queue.</returns>
        public TEntity GetNext(TimeSpan? visibilityTimeout)
        {
            var message = _queue.GetMessage(visibilityTimeout);
            return ToEntity(message);
        }

        /// <summary>
        ///     Converts a <see cref="CloudQueueMessage" /> into an entity of <see cref="TEntity" /> type.
        /// </summary>
        /// <param name="message">Message to convert.</param>
        /// <returns>Converted entity.</returns>
        public static TEntity ToEntity(CloudQueueMessage message)
        {
            if (message == null) return null;

            var json = message.AsString;
            var entity = JsonConvert.DeserializeObject<TEntity>(json);
            entity.Id = message.Id;
            entity.PopReceipt = message.PopReceipt;
            entity.DequeueCount = message.DequeueCount;
            entity.InsertionTime = message.InsertionTime;
            entity.ExpirationTime = message.ExpirationTime;
            return entity;
        }

        /// <summary>
        ///     Converts an entity of <see cref="TEntity" /> type into a <see cref="CloudQueueMessage" />.
        /// </summary>
        /// <param name="entity">Entity to convert.</param>
        /// <returns>Converted message.</returns>
        public static CloudQueueMessage ToCloudQueueMessage(TEntity entity)
        {
            if (entity == null)
                return null;

            var serialized = JsonConvert.SerializeObject(entity);

            if (string.IsNullOrEmpty(entity.Id))
                return new CloudQueueMessage(serialized);

            var message = new CloudQueueMessage(entity.Id, entity.PopReceipt);
            message.SetMessageContent(serialized);
            return message;
        }

        /// <summary>
        ///     Gets the total number of messages into the queue.
        /// </summary>
        /// <returns>Number of messages.</returns>
        public int Count()
        {
            return _queue.ApproximateMessageCount.GetValueOrDefault();
        }
    }
}