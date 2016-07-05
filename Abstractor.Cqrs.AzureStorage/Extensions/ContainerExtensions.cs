using Abstractor.Cqrs.AzureStorage.Blob;
using Abstractor.Cqrs.AzureStorage.Interfaces;
using Abstractor.Cqrs.AzureStorage.Queue;
using Abstractor.Cqrs.AzureStorage.Table;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.AzureStorage.Extensions
{
    /// <summary>
    ///     Extensões do container de inversão de controle.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registra o pacote de integração com o Azure Table Storage.
        /// </summary>
        /// <typeparam name="TContext">Contexto da aplicação que implementa <see cref="AzureTableContext" />.</typeparam>
        /// <param name="container">Container de inversão de controle.</param>
        public static void RegisterAzureTable<TContext>(this IContainer container)
            where TContext : AzureTableContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureTableContext, TContext>();
            container.RegisterScoped(typeof (IAzureTableRepository<>), typeof (AzureTableRepository<>));
        }

        /// <summary>
        ///     Registra o pacote de integração com o Azure Blob Storage.
        /// </summary>
        /// <typeparam name="TContext">Contexto da aplicação que implementa <see cref="AzureBlobContext" />.</typeparam>
        /// <param name="container">Container de inversão de controle.</param>
        public static void RegisterAzureBlob<TContext>(this IContainer container)
            where TContext : AzureBlobContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureBlobContext, TContext>();
            container.RegisterScoped(typeof (IAzureBlobRepository<>), typeof (AzureBlobRepository<>));
        }

        /// <summary>
        ///     Registra o pacote de integração com o Azure Queue Storage.
        /// </summary>
        /// <typeparam name="TContext">Contexto da aplicação que implementa <see cref="AzureQueueContext" />.</typeparam>
        /// <param name="container">Container de inversão de controle.</param>
        public static void RegisterAzureQueue<TContext>(this IContainer container)
            where TContext : AzureQueueContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<AzureQueueContext, TContext>();
            container.RegisterScoped(typeof (IAzureQueueRepository<>), typeof (AzureQueueRepository<>));
        }
    }
}