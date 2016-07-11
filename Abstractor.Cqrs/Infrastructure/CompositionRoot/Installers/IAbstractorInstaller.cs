using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Abstracts the framework installers.
    /// </summary>
    internal interface IAbstractorInstaller
    {
        /// <summary>
        ///     Registers the services into the <see cref="IContainer" />.
        /// </summary>
        /// <param name="container">Inversion of control container.</param>
        /// <param name="settings">Composition settings.</param>
        void RegisterServices(IContainer container, CompositionRootSettings settings);
    }
}