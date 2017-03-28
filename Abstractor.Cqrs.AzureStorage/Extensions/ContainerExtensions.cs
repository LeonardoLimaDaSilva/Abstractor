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
        ///     Registers the Azure Blob Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="IAzureBlobContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureBlob<TContext>(this IContainer container)
            where TContext : IAzureBlobContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<IAzureBlobContext, TContext>();
            container.RegisterScoped(typeof(IAzureBlobRepository<>), typeof(AzureBlobRepository<>));
        }

        /// <summary>
        ///     Registers the Azure Queue Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="IAzureQueueContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureQueue<TContext>(this IContainer container)
            where TContext : IAzureQueueContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<IAzureQueueContext, TContext>();
            container.RegisterScoped(typeof(IAzureQueueRepository<>), typeof(AzureQueueRepository<>));
        }

        /// <summary>
        ///     Registers the Azure Table Storage integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="IAzureTableContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterAzureTable<TContext>(this IContainer container)
            where TContext : IAzureTableContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<IAzureTableContext, TContext>();
            container.RegisterScoped(typeof(IAzureTableRepository<>), typeof(AzureTableRepository<>));
        }
    }
}