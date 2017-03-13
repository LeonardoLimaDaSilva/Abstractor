using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Domain;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

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

                interfaces = ExcludeIfHasMultipleImplementations(interfaces);

                foreach (var i in interfaces)
                    container.RegisterTransient(i, type);
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
                           (i.IsGenericType && (i.GetGenericTypeDefinition() != typeof(ICommandHandler<>)) &&
                            (i.GetGenericTypeDefinition() != typeof(IQueryHandler<,>)) &&
                            (i.GetGenericTypeDefinition() != typeof(IQueryAsyncHandler<,>)) &&
                            (i.GetGenericTypeDefinition() != typeof(IDomainEventHandler<>)) &&
                            (i.GetGenericTypeDefinition() != typeof(IApplicationEventHandler<>)))
                           || !i.IsGenericType);
        }

        /// <summary>
        ///     Excludes the IFileRepository from list if there are multiple concrete types implementing it.
        /// </summary>
        /// <param name="interfaces"></param>
        /// <returns></returns>
        private static List<Type> ExcludeIfHasMultipleImplementations(List<Type> interfaces)
        {
            if ((interfaces.Count > 1) && interfaces.Any(i => i == typeof(IFileRepository)))
                interfaces = interfaces.Where(i => i != typeof(IFileRepository)).ToList();

            return interfaces;
        }
    }
}