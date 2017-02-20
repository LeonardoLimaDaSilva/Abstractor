using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Middleware to intercept the request if the token is expired.
    /// </summary>
    internal class ExpirationTokenMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        /// <summary>
        ///     Constructs the middleware.
        /// </summary>
        /// <param name="next">The next task.</param>
        public ExpirationTokenMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        /// <summary>
        ///     Intercepts the request and verify if the expired token information is present on OwinContext.
        ///     If expired, returns the 401 status code with the "Token expired" reason phrase.
        /// </summary>
        /// <param name="arg">Current task arguments.</param>
        /// <returns>Next task, or short circuits if token is expired.</returns>
        public async Task Invoke(IDictionary<string, object> arg)
        {
            var ctx = new OwinContext(arg);
            if (ctx.Get<bool>("custom.ExpiredToken"))
            {
                ctx.Response.OnSendingHeaders(state =>
                       {
                           var response = (OwinResponse) state;

                           response.StatusCode = 401;
                           response.ReasonPhrase = "Token expired";
                       },
                       ctx.Response);

                await Task.FromResult(0);
            }

            await _next(arg);
        }
    }
}