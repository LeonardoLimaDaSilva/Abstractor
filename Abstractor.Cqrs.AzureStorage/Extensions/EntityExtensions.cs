using System;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Attributes;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensions for the Azure Storage entities.
    /// </summary>
    internal static class EntityExtensions
    {
        /// <summary>
        ///     Gets the container name using the <see cref="AzureContainerAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Container name.</returns>
        public static string GetContainerName(this Type type)
        {
            var attributes =
                (AzureContainerAttribute[]) type.GetCustomAttributes(typeof(AzureContainerAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name.ToLowerInvariant()
                : type.Name.ToLowerInvariant();
        }

        /// <summary>
        ///     Gets the queue name using the <see cref="AzureQueueAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Table name.</returns>
        public static string GetQueueName(this Type type)
        {
            var attributes = (AzureQueueAttribute[]) type.GetCustomAttributes(typeof(AzureQueueAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name.ToLowerInvariant()
                : type.Name.ToLowerInvariant();
        }

        /// <summary>
        ///     Gets the table name using the <see cref="AzureTableAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Table name.</returns>
        public static string GetTableName(this Type type)
        {
            var attributes = (AzureTableAttribute[]) type.GetCustomAttributes(typeof(AzureTableAttribute), false);
            return attributes.Length > 0
                ? attributes.First().Name
                : type.Name;
        }
    }
}