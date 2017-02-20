using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Extends the IAppBuilder interface to provide the expiration token middleware activator.
    /// </summary>
    internal static class ExpirationTokenMiddlewareExtension
    {
        /// <summary>
        ///     Activates the expiration token middleware.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IAppBuilder UseExpirationToken(this IAppBuilder builder)
        {
            return builder.Use(
                new Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>(
                    next => new ExpirationTokenMiddleware(next).Invoke));
        }
    }
}