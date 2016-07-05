using System;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    public class AzureQueueMessage
    {
        public string Id { get; set; }

        public string PopReceipt { get; set; }

        public int DequeueCount { get; set; }

        public DateTimeOffset? InsertionTime { get; set; }

        public DateTimeOffset? ExpirationTime { get; set; }
    }
}