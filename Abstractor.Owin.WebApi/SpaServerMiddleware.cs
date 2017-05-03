using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.StaticFiles;

namespace Abstractor.Owin.WebApi
{
    internal class SpaServerMiddleware
    {
        private readonly StaticFileMiddleware _innerMiddleware;
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly SpaServerOptions _options;

        /// <summary>
        ///     Constructs the middleware.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public SpaServerMiddleware(Func<IDictionary<string, object>, Task> next, SpaServerOptions options)
        {
            _next = next;
            _options = options;
            _innerMiddleware = new StaticFileMiddleware(next, options.FileServerOptions.StaticFileOptions);
        }

        /// <summary>
        ///     If resource is not found and Html5 mode is enabled the entry path is setted to the request path.
        /// </summary>
        /// <param name="arg">Current task arguments.</param>
        /// <returns></returns>
        public async Task Invoke(IDictionary<string, object> arg)
        {
            await _innerMiddleware.Invoke(arg);

            if ((int) arg["owin.ResponseStatusCode"] == 404 && _options.Html5Mode)
            {
                arg["owin.RequestPath"] = _options.EntryPath.Value;
                await _next.Invoke(arg);
            }
        }
    }
}