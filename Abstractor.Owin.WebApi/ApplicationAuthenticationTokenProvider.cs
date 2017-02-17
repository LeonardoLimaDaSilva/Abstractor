using System;
using Microsoft.Owin.Security.Infrastructure;

namespace Abstractor.Owin.WebApi
{
    public class ApplicationAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket?.Properties.ExpiresUtc != null && context.Ticket.Properties.ExpiresUtc.Value.LocalDateTime < DateTime.Now)
                context.OwinContext.Set("custom.ExpiredToken", true);
        }
    }
}