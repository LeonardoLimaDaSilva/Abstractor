using System;
using Microsoft.Owin.Security.Infrastructure;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Provides a method for verifying the authentication token expiration.
    /// </summary>
    internal class AuthenticationTokenExpiryProvider : AuthenticationTokenProvider
    {
        /// <summary>
        ///     Verifies the authentication token expiration.
        /// </summary>
        /// <param name="context"></param>
        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket?.Properties.ExpiresUtc != null &&
                context.Ticket.Properties.ExpiresUtc.Value.LocalDateTime < DateTime.Now)
                context.OwinContext.Set("custom.ExpiredToken", true);
        }
    }
}