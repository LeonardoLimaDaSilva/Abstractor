using Abstractor.Cqrs.Interfaces.CompositionRoot;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot.Installers
{
    /// <summary>
    ///     Encapsula a infraestrutura do framework e registra os serviços no container de inversão de controle.
    /// </summary>
    internal interface IShopDeliveryInstaller
    {
        /// <summary>
        ///     Registra os serviços no <see cref="IContainer" />.
        /// </summary>
        /// <param name="container">Container de inversão de controle.</param>
        /// <param name="settings">Configurações de composição.</param>
        void RegisterServices(IContainer container, CompositionRootSettings settings);
    }
}