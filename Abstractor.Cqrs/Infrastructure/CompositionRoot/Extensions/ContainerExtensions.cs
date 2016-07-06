using System;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions
{
    /// <summary>
    ///     Raiz de composição do framework.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        ///     Registra o framework no container de inversão de controle.
        /// </summary>
        /// <param name="container">O container de inversão de controle.</param>
        /// <param name="settings">Configurações de composição.</param>
        public static void RegisterAbstractor(this IContainer container, Action<CompositionRootSettings> settings)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(settings, nameof(settings));

            var crs = new CompositionRootSettings();
            settings.Invoke(crs);

            container.AllowResolvingFuncFactories();
            container.Register(() => crs);
            container.RegisterAbstractorInstallers(crs);
        }

        /// <summary>
        ///     Obtém as instâncias definidas nos instaladores do framework e registra os serviços.
        /// </summary>
        /// <param name="container">Container de inversão de controle.</param>
        /// <param name="settings">Configurações de composição.</param>
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