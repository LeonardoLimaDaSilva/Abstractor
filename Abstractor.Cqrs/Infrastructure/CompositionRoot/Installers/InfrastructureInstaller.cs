using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Persistence;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Events;

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
            container.RegisterLazySingleton<ILogger, EmptyLogger>();
            container.RegisterLazySingleton<ILoggerSerializer, JsonLoggerSerializer>();
            container.RegisterLazySingleton<IStopwatch, DefaultStopwatch>();
            container.RegisterLazySingleton<IValidator, DataAnnotationsValidator>();
            container.RegisterLazySingleton<IClock, SystemClock>();
            container.RegisterLazySingleton<IAttributeFinder, AttributeFinder>();

            if (settings.ApplicationTypes == null) return;

            // Registers all the implementations from the application
            foreach (var type in settings.ApplicationTypes)
            {
                var interfaces = ExcludeEventHandlersInterfaces(type).ToList();
                if (interfaces.Any())
                    container.RegisterTransient(interfaces.Last(), type);
            }
        }

        /// <summary>
        ///     Excludes the event handlers interfaces because they are already registered on EventInstaller.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<Type> ExcludeEventHandlersInterfaces(Type type)
        {
            return type.GetInterfaces().Where(i =>
                (i.IsGenericType && (
                    i.GetGenericTypeDefinition() != typeof (IDomainEventHandler<>) &&
                    i.GetGenericTypeDefinition() != typeof (IApplicationEventHandler<>)
                    ))
                || !i.IsGenericType);
        }
    }
}