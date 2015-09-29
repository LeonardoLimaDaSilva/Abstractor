using System;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Persistence;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    internal sealed class InfrastructureInstaller : IAbstractorInstaller
    {
        public void RegisterServices(Container container, CompositionRootSettings settings)
        {
            // Registra as implementações padrão para serem utilizadas quando não forem explicitamente registradas
            var uowRegistration = new Lazy<Registration>(() => Lifestyle.Singleton.CreateRegistration<IUnitOfWork, EmptyUnitOfWork>(container));
            var loggerRegistration = new Lazy<Registration>(() => Lifestyle.Singleton.CreateRegistration<ILogger, EmptyLogger>(container));
            var validatorRegistration = new Lazy<Registration>(() => Lifestyle.Singleton.CreateRegistration<IValidator, EmptyValidator>(container));
            var clockRegistration = new Lazy<Registration>(() => Lifestyle.Singleton.CreateRegistration<IClock, SystemClock>(container));

            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType == typeof(IUnitOfWork)) e.Register(uowRegistration.Value);
                if (e.UnregisteredServiceType == typeof(ILogger)) e.Register(loggerRegistration.Value);
                if (e.UnregisteredServiceType == typeof(IValidator)) e.Register(validatorRegistration.Value);
                if (e.UnregisteredServiceType == typeof(IClock)) e.Register(clockRegistration.Value);
            };

            // Registra as implementações da camada de persistência
            foreach (var type in settings.PersistenceTypes)
                container.Register(type.GetInterfaces().First(), type, Lifestyle.Scoped);
        }
    }
}