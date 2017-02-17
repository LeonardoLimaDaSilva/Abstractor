using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;

namespace Abstractor.Owin.WebApi
{
    public class SpaServerOptions
    {
        public PathString EntryPath { get; set; }

        public FileServerOptions FileServerOptions { get; set; }

        public bool Html5Mode => EntryPath.HasValue;

        public SpaServerOptions()
        {
            FileServerOptions = new FileServerOptions();
            EntryPath = PathString.Empty;
        }
    }
}