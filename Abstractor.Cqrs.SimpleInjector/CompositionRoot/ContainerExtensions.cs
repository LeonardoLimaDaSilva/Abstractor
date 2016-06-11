using System;
using System.Linq;
using System.Linq.Expressions;
using Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers;
using Abstractor.Cqrs.SimpleInjector.Extensions;
using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot
{
    /// <summary>
    /// Raiz de composição do framework utilizando o Simple Injector.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Registra o framework no container do Simple Injector.
        /// </summary>
        /// <param name="container">O container do Simple Injector.</param>
        /// <param name="settings">Configurações de composição.</param>
        public static void RegisterAbstractor(this Container container, Action<CompositionRootSettings> settings)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var crs = new CompositionRootSettings();
            settings.Invoke(crs);

            container.Options.AllowResolvingFuncFactories();

            container.Register(() => crs);
            container.RegisterAbstractorInstallers(crs);
        }

        /// <summary>
        /// Obtém as instâncias definidas nos instaladores do framework e registra os serviços.
        /// </summary>
        /// <param name="container">Container do Simple Injector.</param>
        /// <param name="settings">Configurações de composição.</param>
        internal static void RegisterAbstractorInstallers(this Container container, CompositionRootSettings settings)
        {
            var packages = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                           from type in assembly.GetSafeTypes()
                           where typeof(IAbstractorInstaller).IsAssignableFrom(type)
                           where !type.IsAbstract
                           select (IAbstractorInstaller)Activator.CreateInstance(type);

            packages.ToList().ForEach(p => p.RegisterServices(container, settings));
        }

        /// <summary>
        /// Permite que delegates sejam resolvidos pelo container.
        /// </summary>
        /// <param name="options">Opções do container.</param>
        internal static void AllowResolvingFuncFactories(this ContainerOptions options)
        {
            options.Container.ResolveUnregisteredType += (s, e) =>
            {
                var type = e.UnregisteredServiceType;

                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Func<>)) return;

                var serviceType = type.GetGenericArguments().First();
                var registration = options.Container.GetRegistration(serviceType, true);
                var funcType = typeof(Func<>).MakeGenericType(serviceType);

                // Constrói o delegate e o registra no container
                var factoryDelegate = Expression.Lambda(funcType, registration.BuildExpression()).Compile();
                e.Register(Expression.Constant(factoryDelegate));
            };
        }
    }
}
