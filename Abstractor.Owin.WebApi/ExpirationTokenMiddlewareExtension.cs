using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;

namespace Abstractor.Owin.WebApi
{
    public static class ExpirationTokenMiddlewareExtension
    {
        public static IAppBuilder UseExpirationToken(this IAppBuilder builder)
        {
            return builder.Use(
                new Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>(
                    next => new ExpirationTokenMiddleware(next).Invoke));
        }
    }
}