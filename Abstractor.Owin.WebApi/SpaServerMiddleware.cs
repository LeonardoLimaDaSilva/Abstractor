using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.StaticFiles;

namespace Abstractor.Owin.WebApi
{
    public class SpaServerMiddleware
    {
        private readonly StaticFileMiddleware _innerMiddleware;
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly SpaServerOptions _options;

        public SpaServerMiddleware(Func<IDictionary<string, object>, Task> next, SpaServerOptions options)
        {
            _next = next;
            _options = options;
            _innerMiddleware = new StaticFileMiddleware(next, options.FileServerOptions.StaticFileOptions);
        }

        public async Task Invoke(IDictionary<string, object> arg)
        {
            await _innerMiddleware.Invoke(arg);

            if ((int)arg["owin.ResponseStatusCode"] == 404 && _options.Html5Mode)
            {
                arg["owin.RequestPath"] = _options.EntryPath.Value;
                await _next.Invoke(arg);
            }
        }
    }
}