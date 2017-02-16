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
        /// <param name="compositionSettings">Composition settings.</param>
        /// <param name="globalSettings">Global settings.</param>
        public static void RegisterAbstractor(
            this IContainer container,
            Action<CompositionRootSettings> compositionSettings,
            Action<GlobalSettings> globalSettings = null)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(compositionSettings, nameof(compositionSettings));

            var crs = new CompositionRootSettings();
            compositionSettings.Invoke(crs);

            container.AllowResolvingFuncFactories();
            container.RegisterSingleton(() => crs);
            container.RegisterAbstractorInstallers(crs);

            if (globalSettings == null)
                globalSettings = gs => { };

            var gsInstance = new GlobalSettings();

            globalSettings.Invoke(gsInstance);
            container.RegisterSingleton(() => gsInstance);
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
                where typeof(IAbstractorInstaller).IsAssignableFrom(type)
                where !type.IsAbstract
                select (IAbstractorInstaller) Activator.CreateInstance(type);

            packages.ToList().ForEach(p => p.RegisterServices(container, settings));
        }
    }
}