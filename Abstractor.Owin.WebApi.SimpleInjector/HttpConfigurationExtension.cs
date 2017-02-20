using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Abstractor.Owin.WebApi.SimpleInjector
{
    /// <summary>
    ///     Extends the HttpConfiguration class.
    /// </summary>
    public static class HttpConfigurationExtension
    {
        /// <summary>
        ///     Registers the Web Api controllers and defines the SimpleInjector as the dependency resolver.
        /// </summary>
        /// <param name="httpConfiguration"></param>
        public static void UseSimpleInjector(this HttpConfiguration httpConfiguration)
        {
            ContainerFactory.Container.RegisterWebApiControllers(httpConfiguration);
            ContainerFactory.Container.Verify();

            httpConfiguration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(ContainerFactory.Container);
        }
    }
}