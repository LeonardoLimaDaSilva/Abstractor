using System;
using System.Linq;
using Abstractor.Cqrs.AzureStorage.Attributes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensions for the Azure Storage entities.
    /// </summary>
    internal static class EntityExtensions
    {
        /// <summary>
        ///     Gets the cache control value defined for the entity.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Cache control value.</returns>
        public static string GetBlobCacheControl(this Type type)
        {
            var attribute =
                ((AzureBlobCacheControlAttribute[]) type.GetCustomAttributes(typeof(AzureBlobCacheControlAttribute),
                    false))
                .SingleOrDefault();

            return attribute?.Value.ToLowerInvariant();
        }

        /// <summary>
        ///     Gets the container name using the <see cref="AzureContainerAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Container name.</returns>
        public static string GetContainerName(this Type type)
        {
            var attribute =
                ((AzureContainerAttribute[]) type.GetCustomAttributes(typeof(AzureContainerAttribute), false))
                .SingleOrDefault();

            return attribute?.Name.ToLowerInvariant() ?? type.Name.ToLowerInvariant();
        }

        /// <summary>
        ///     Gets the public access type using the <see cref="AzureContainerAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>The public access type.</returns>
        public static BlobContainerPublicAccessType GetPublicAccessType(this Type type)
        {
            var attribute =
                ((AzureContainerAttribute[]) type.GetCustomAttributes(typeof(AzureContainerAttribute), false))
                .SingleOrDefault();

            return attribute?.PublicAccessType ?? BlobContainerPublicAccessType.Off;
        }

        /// <summary>
        ///     Gets the queue name using the <see cref="AzureQueueAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Table name.</returns>
        public static string GetQueueName(this Type type)
        {
            var attribute =
                ((AzureQueueAttribute[]) type.GetCustomAttributes(typeof(AzureQueueAttribute), false))
                .SingleOrDefault();

            return attribute?.Name.ToLowerInvariant() ?? type.Name.ToLowerInvariant();
        }

        /// <summary>
        ///     Gets the table name using the <see cref="AzureTableAttribute" /> decorated in the entity class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        /// <returns>Table name.</returns>
        public static string GetTableName(this Type type)
        {
            var attribute =
                ((AzureTableAttribute[]) type.GetCustomAttributes(typeof(AzureTableAttribute), false))
                .SingleOrDefault();

            return attribute?.Name ?? type.Name;
        }
    }
}