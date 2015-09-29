using SimpleInjector;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot.Installers
{
    /// <summary>
    /// Encapsula a infraestrutura do Abstractor e registra os serviços no container do Simple Injector.
    /// </summary>
    internal interface IAbstractorInstaller
    {
        /// <summary>
        /// Registra os serviços no <see cref="Container"/>.
        /// </summary>
        /// <param name="container">Container do Simple Injector.</param>
        /// <param name="settings">Configurações de composição.</param>
        void RegisterServices(Container container, CompositionRootSettings settings);
    }
}
