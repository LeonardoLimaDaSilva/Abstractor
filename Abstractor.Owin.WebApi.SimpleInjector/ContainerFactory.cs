using Abstractor.Cqrs.SimpleInjector.Adapters;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Abstractor.Owin.WebApi.SimpleInjector
{
    /// <summary>
    ///     Factory for SimpleInjector container adapter.
    /// </summary>
    public static class ContainerFactory
    {
        private static Container _container;

        /// <summary>
        ///     Singleton instance of SimpleInjector underlying container.
        /// </summary>
        public static Container Container => _container ?? (_container = new Container());

        /// <summary>
        ///     Creates a new instance of container adapter with the default scoped Web Api Lifestyle.
        /// </summary>
        /// <returns>An adapter.</returns>
        public static ContainerAdapter CreateAdapter()
        {
            Container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();
            return new ContainerAdapter(Container);
        }
    }
}