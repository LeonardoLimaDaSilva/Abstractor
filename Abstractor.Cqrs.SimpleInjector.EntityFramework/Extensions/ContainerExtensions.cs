using System;
using System.Data.Entity;
using System.Reflection;
using Abstractor.Cqrs.Interfaces.Persistence;
using Abstractor.Cqrs.SimpleInjector.EntityFramework.ModelCreation;
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
        /// <param name="infrastructureAssemblies">Local dos assemblies da camada de infraestrutura.</param>
        public static void RegisterEntityFramework<TContext>(this Container container, Assembly[] infrastructureAssemblies) where TContext : DbContext
        {
            if (container == null) 
                throw new ArgumentNullException(nameof(container));

            container.Register<ICreateDbModel>(() => new DefaultDbModelCreator(infrastructureAssemblies), Lifestyle.Singleton);
            container.Register<DbContext, TContext>(Lifestyle.Scoped);
            container.Register<IUnitOfWork, EntityFrameworkUnitOfWork>(Lifestyle.Scoped);
            container.Register(typeof (IRepository<>), typeof (EntityFrameworkRepository<>), Lifestyle.Scoped);
        }
    }
}
