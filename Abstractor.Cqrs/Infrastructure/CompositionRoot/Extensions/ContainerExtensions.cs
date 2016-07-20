using System;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions
{
    /// <summary>
    ///     Extensions for the inversion of control container abstraction.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registers the framework dependencies into the inversion of control container.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        /// <param name="settings">Composition settings.</param>
        public static void RegisterAbstractor(this IContainer container, Action<CompositionRootSettings> settings)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            var crs = new CompositionRootSettings();
            settings.Invoke(crs);

            container.AllowResolvingFuncFactories();
            container.RegisterTransient(() => crs);
            container.RegisterAbstractorInstallers(crs);
        }

        /// <summary>
        ///     Discovers all framework installers and registers the services.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        /// <param name="settings">Composition settings.</param>
        private static void RegisterAbstractorInstallers(
            this IContainer container,
            CompositionRootSettings settings)
        {
            var packages = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetSafeTypes()
                where typeof (IAbstractorInstaller).IsAssignableFrom(type)
                where !type.IsAbstract
                select (IAbstractorInstaller) Activator.CreateInstance(type);

            packages.ToList().ForEach(p => p.RegisterServices(container, settings));
        }
    }
}