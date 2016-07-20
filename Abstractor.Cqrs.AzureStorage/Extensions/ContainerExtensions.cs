using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.AzureStorage.Queue;
using Abstractor.Cqrs.AzureStorage.Table;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensions for the inversion of control container abstraction.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registers the Azure Table Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="AzureTableContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureTable<TContext>(this IContainer container)
            where TContext : AzureTableContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureTableContext, TContext>();
            container.RegisterScoped(typeof (IAzureTableRepository<>), typeof (AzureTableRepository<>));
        }

        /// <summary>
        ///     Registers the Azure Blob Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="AzureBlobContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureBlob<TContext>(this IContainer container)
            where TContext : AzureBlobContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureBlobContext, TContext>();
            container.RegisterScoped(typeof (IAzureBlobRepository<>), typeof (AzureBlobRepository<>));
        }

        /// <summary>
        ///     Registers the Azure Queue Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="AzureQueueContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureQueue<TContext>(this IContainer container)
            where TContext : AzureQueueContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureQueueContext, TContext>();
            container.RegisterScoped(typeof (IAzureQueueRepository<>), typeof (AzureQueueRepository<>));
        }
    }
}