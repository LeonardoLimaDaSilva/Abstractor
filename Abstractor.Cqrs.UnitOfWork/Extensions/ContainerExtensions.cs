using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.UnitOfWork.Extensions
{
    /// <summary>
    ///     Extensions for the inversion of control container abstraction.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registers the Unit of Work integration package.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        public static void RegisterUnitOfWork(this IContainer container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.RegisterScoped<IUnitOfWork, Persistence.UnitOfWork>();
        }
    }
}