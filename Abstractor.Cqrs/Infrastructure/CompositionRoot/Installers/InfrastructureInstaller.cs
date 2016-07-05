using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    internal sealed class InfrastructureInstaller : IShopDeliveryInstaller
    {
        public void RegisterServices(IContainer container, CompositionRootSettings settings)
        {
            // Registra as implementações padrão para serem utilizadas quando não forem explicitamente registradas
            container.RegisterLazySingleton<IUnitOfWork, EmptyUnitOfWork>();
            container.RegisterLazySingleton<ILogger, EmptyLogger>();
            container.RegisterLazySingleton<IValidator, DataAnnotationsValidator>();
            container.RegisterLazySingleton<IClock, SystemClock>();

            // Registra as implementações da camada de persistência
            if (settings.PersistenceTypes != null)
                foreach (var type in settings.PersistenceTypes)
                    container.RegisterScoped(type.GetInterfaces().Last(), type);
            
            if (settings.ApplicationTypes == null) return;
            
            // Registra as implementações da camada de aplicação
            foreach (var type in settings.ApplicationTypes)
                container.RegisterTransient(type.GetInterfaces().First(), type);
        }
    }
}