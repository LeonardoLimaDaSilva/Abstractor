using System;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces;
using Abstractor.Cqrs.SimpleInjector.AzureStorage.Persistence;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Extensions
{
    /// <summary>
    /// Extensões do container do Simple Injector.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Registra o pacote de integração com o Azure Storage no container do Simple Injector.
        /// </summary>
        /// <typeparam name="TContext">TsContext da aplicação.</typeparam>
        /// <param name="container">Container do Simple Injector.</param>
        /// <param name="compositeUnitOfWork">Registra o AzureStorageUnitOfWork apenas se não estiver usando um composite.</param>
        public static void RegisterAzureStorage<TContext>(this Container container, bool compositeUnitOfWork = false)
            where TContext : TsContext
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            container.Register<TsContext, TContext>(Lifestyle.Scoped);

            if (!compositeUnitOfWork)
                container.Register<IUnitOfWork, AzureTableUnitOfWork>(Lifestyle.Scoped);

            container.Register(typeof(IAzureTableRepository<>), typeof(AzureTableRepository<>), Lifestyle.Scoped);
        }
    }
}
