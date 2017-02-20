using System;
using System.Web.Http;
using Newtonsoft.Json;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Available options for Web Api configuration.
    /// </summary>
    public class WebApiOptions
    {
        /// <summary>
        ///     The period of time the access token remains valid after being issued. The default is one day.
        /// </summary>
        public TimeSpan AccessTokenExpireTimeSpan { get; set; }

        /// <summary>
        ///     Specifies how dates are formatted when writing JSON. The default is IsoDateFormat.
        /// </summary>
        public DateFormatHandling DateFormatHandling { get; set; }

        /// <summary>
        ///     Specifies how to treat the time value when converting between string and DateTime. The default is Utc.
        /// </summary>
        public DateTimeZoneHandling DateTimeZoneHandling { get; set; }

        /// <summary>
        ///     Specifies whether error details should be included in error messages. The default is always include.
        /// </summary>
        public IncludeErrorDetailPolicy IncludeErrorDetailPolicy { get; set; }

        /// <summary>
        ///     The file system path used to locate resources.
        /// </summary>
        public string StaticFileSystemPath { get; set; }

        /// <summary>
        ///     The request path client applications communicate with directly as part of the OAuth protocol.
        /// </summary>
        public string TokenEndpointPath { get; set; }

        /// <summary>
        ///     Constructs the option with default values.
        /// </summary>
        public WebApiOptions()
        {
            TokenEndpointPath = "/token";
            AccessTokenExpireTimeSpan = TimeSpan.FromDays(1);
            IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            StaticFileSystemPath = "./wwwroot";
        }
    }
}