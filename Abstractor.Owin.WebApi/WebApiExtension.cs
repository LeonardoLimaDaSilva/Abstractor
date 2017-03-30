using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Extends the IAppBuilder interface to provide the Web Api activator.
    /// </summary>
    public static class WebApiExtension
    {
        /// <summary>
        ///     Activates the common Web Api configuration.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authorizationServerProvider"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static HttpConfiguration UseAbstractorWebApi(
            this IAppBuilder builder,
            IOAuthAuthorizationServerProvider authorizationServerProvider,
            WebApiOptions options = null)
        {
            if (options == null)
                options = new WebApiOptions();

            builder.UseCors(CorsOptions.AllowAll);

            builder.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString(options.TokenEndpointPath),
                AccessTokenExpireTimeSpan = options.AccessTokenExpireTimeSpan,
                Provider = authorizationServerProvider
            });

            builder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                AccessTokenProvider = new AuthenticationTokenExpiryProvider()
            });

            builder.UseExpirationToken();

            var httpConfiguration = new HttpConfiguration
            {
                IncludeErrorDetailPolicy = options.IncludeErrorDetailPolicy
            };

            httpConfiguration.MapHttpAttributeRoutes();

            httpConfiguration.Filters.Add(new ValidateModelFilterAttribute());

            var jsonFormatter = httpConfiguration.Formatters.OfType<JsonMediaTypeFormatter>().First();

            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.DateFormatHandling = options.DateFormatHandling;
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = options.DateTimeZoneHandling;

            httpConfiguration.Formatters.Add(new GenericFileMediaTypeFormatter());

            builder.UseWebApi(httpConfiguration);

            builder.UseFileServer(
                new FileServerOptions
                {
                    RequestPath = new PathString(string.Empty),
                    FileSystem = new PhysicalFileSystem(options.StaticFileSystemPath),
                    EnableDefaultFiles = true
                });

            builder.UseStageMarker(PipelineStage.MapHandler);

            return httpConfiguration;
        }
    }
}