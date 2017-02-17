using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Abstractor.Owin.WebApi
{
    public class ExpirationTokenMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public ExpirationTokenMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> arg)
        {
            var ctx = new OwinContext(arg);
            if (ctx.Get<bool>("custom.ExpiredToken"))
            {
                ctx.Response.OnSendingHeaders(state =>
                {
                    var response = (OwinResponse)state;

                    response.StatusCode = 401;
                    response.ReasonPhrase = "Token expired";

                }, ctx.Response);

                await Task.FromResult(0);
            }

            await _next(arg);
        }
    }
}