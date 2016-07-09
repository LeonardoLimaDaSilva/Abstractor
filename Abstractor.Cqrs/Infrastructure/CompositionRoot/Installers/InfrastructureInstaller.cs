using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Registers the default services and the persistence and application types.
    /// </summary>
    internal sealed class InfrastructureInstaller : IAbstractorInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            // Registers the default implementations
            container.RegisterLazySingleton<IUnitOfWork, EmptyUnitOfWork>();
            container.RegisterLazySingleton<ILogger, EmptyLogger>();
            container.RegisterLazySingleton<ILoggerSerializer, JsonLoggerSerializer>();
            container.RegisterLazySingleton<IStopwatch, DefaultStopwatch>();
            container.RegisterLazySingleton<IValidator, DataAnnotationsValidator>();
            container.RegisterLazySingleton<IClock, SystemClock>();

            // Registers all the implementations from the persistence layer
            if (settings.PersistenceTypes != null)
                foreach (var type in settings.PersistenceTypes)
                    container.RegisterScoped(type.GetInterfaces().Last(), type);

            if (settings.ApplicationTypes == null) return;

            // Registers all the implementations from the application layer
            foreach (var type in settings.ApplicationTypes)
                container.RegisterTransient(type.GetInterfaces().First(), type);
        }
    }
}