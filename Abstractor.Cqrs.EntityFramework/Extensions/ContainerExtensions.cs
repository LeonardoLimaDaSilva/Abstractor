using System.Data.Entity;
using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.EntityFramework.Persistence;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    /// <summary>
    ///     Extensões do container de inversão de controle.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registra o pacote de integração com o Entity Framework.
        /// </summary>
        /// <typeparam name="TContext">DbContext da aplicação.</typeparam>
        /// <param name="container">Container de inversão de controle.</param>
        /// <param name="customUnitOfWork">Registra o EntityFrameworkUnitOfWork apenas se não estiver usando um customizado.</param>
        public static void RegisterEntityFramework<TContext>(this IContainer container, bool customUnitOfWork = false)
            where TContext : DbContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<DbContext, TContext>();

            if (!customUnitOfWork)
                container.RegisterScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();

            container.RegisterScoped(typeof (IEntityFrameworkRepository<>), typeof (EntityFrameworkRepository<>));
        }
    }
}