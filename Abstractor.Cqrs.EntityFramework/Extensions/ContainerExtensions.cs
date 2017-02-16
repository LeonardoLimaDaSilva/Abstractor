using Abstractor.Cqrs.EntityFramework.Interfaces;
using Abstractor.Cqrs.EntityFramework.Persistence;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    /// <summary>
    ///     Extensions for the inversion of control container abstraction.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registers the Entity Framework integration packages.
        /// </summary>
        /// <typeparam name="TContext">Application context that implements <see cref="IEntityFrameworkContext" />.</typeparam>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterEntityFramework<TContext>(this IContainer container)
            where TContext : IEntityFrameworkContext
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<IEntityFrameworkContext, TContext>();
            container.RegisterLazyScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();
            container.RegisterScoped(typeof(IEntityFrameworkRepository<>), typeof(EntityFrameworkRepository<>));
        }
    }
}