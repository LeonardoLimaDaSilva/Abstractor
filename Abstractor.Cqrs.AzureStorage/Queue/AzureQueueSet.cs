using System;
using Abstractor.Cqrs.AzureStorage.Extensions;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    public sealed class AzureQueueSet<TEntity> : BaseDataSet<TEntity> where TEntity : AzureQueueMessage, new()
    {
        private readonly CloudQueue _queue;

        public AzureQueueSet(string connectionString) : base(false)
        {
            var queueClient = CloudStorageAccount.Parse(connectionString).CreateCloudQueueClient();
            var queueName = typeof (TEntity).GetQueueName();
            _queue = queueClient.GetQueueReference(queueName);
            _queue.CreateIfNotExists();
        }

        protected override void InsertEntity(TEntity entity)
        {
            _queue.AddMessage(ToCloudQueueMessage(entity));
        }

        protected override void DeleteEntity(TEntity entity)
        {
            _queue.DeleteMessage(ToCloudQueueMessage(entity));
        }

        protected override void UpdateEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override TEntity Get(TEntity entity)
        {
            var message = _queue.GetMessage();
            return ToEntity(message);
        }

        public TEntity GetNext(TimeSpan? visibilityTimeout)
        {
            var message = _queue.GetMessage(visibilityTimeout);
            return ToEntity(message);
        }

        public static TEntity ToEntity(CloudQueueMessage message)
        {
            if (message == null)
                return null;

            var json = message.AsString;
            var entity = JsonConvert.DeserializeObject<TEntity>(json);
            entity.Id = message.Id;
            entity.PopReceipt = message.PopReceipt;
            entity.DequeueCount = message.DequeueCount;
            entity.InsertionTime = message.InsertionTime;
            entity.ExpirationTime = message.ExpirationTime;
            return entity;
        }

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

        public int Count()
        {
            return _queue.ApproximateMessageCount.GetValueOrDefault();
        }
    }
}