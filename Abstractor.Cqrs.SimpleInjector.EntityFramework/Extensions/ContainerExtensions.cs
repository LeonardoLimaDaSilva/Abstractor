using System;
using System.Data.Entity;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.EntityFramework.Interfaces;
using Abstractor.Cqrs.SimpleInjector.EntityFramework.Persistence;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.EntityFramework.Extensions
{
    /// <summary>
    /// Extensões do container do Simple Injector.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Registra o pacote de integração com o Entity Framework no container do Simple Injector.
        /// </summary>
        /// <typeparam name="TContext">DbContext da aplicação.</typeparam>
        /// <param name="container">Container do Simple Injector.</param>
        /// <param name="compositeUnitOfWork">Registra o EntityFrameworkUnitOfWork apenas se não estiver usando um composite.</param>
        public static void RegisterEntityFramework<TContext>(this Container container, bool compositeUnitOfWork = false)
            where TContext : DbContext
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            container.Register<DbContext, TContext>(Lifestyle.Scoped);

            if (!compositeUnitOfWork)
                container.Register<IUnitOfWork, EntityFrameworkUnitOfWork>(Lifestyle.Scoped);

            container.Register(typeof(IEntityFrameworkRepository<>), typeof(EntityFrameworkRepository<>), Lifestyle.Scoped);
        }
    }
}
