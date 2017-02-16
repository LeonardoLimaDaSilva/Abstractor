using System;

namespace Abstractor.Cqrs.AzureStorage.Queue
{
    /// <summary>
    ///     Minimal representation of an Azure Queue Message.
    /// </summary>
    public class AzureQueueMessage
    {
        /// <summary>
        ///     Gets the number of times this message has been dequeued.
        /// </summary>
        public int DequeueCount { get; set; }

        /// <summary>
        ///     Gets the time that the message expires.
        /// </summary>
        public DateTimeOffset? ExpirationTime { get; set; }

        /// <summary>
        ///     Gets the message Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Gets the time that the message was added to the queue.
        /// </summary>
        public DateTimeOffset? InsertionTime { get; set; }

        /// <summary>
        ///     Gets the message's pop receipt.
        /// </summary>
        public string PopReceipt { get; set; }
    }
}