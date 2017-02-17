using System;
using System.Web.Http;
using Newtonsoft.Json;

namespace Abstractor.Owin.WebApi
{
    public class WebApiOptions
    {
        public string TokenEndpointPath { get; set; }

        public TimeSpan AccessTokenExpireTimeSpan { get; set; }

        public IncludeErrorDetailPolicy IncludeErrorDetailPolicy { get; set; }

        public DateFormatHandling DateFormatHandling { get; set; }

        public DateTimeZoneHandling DateTimeZoneHandling { get; set; }

        public string StaticFileSystemPath { get; set; }

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