using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Abstractor.Owin.WebApi
{
    public static class SpaExtension
    {
        public static IAppBuilder UseSpa(
            this IAppBuilder builder,
            string entryPath = "/index.html",
            string fileSystemPath = "./wwwroot")
        {
            builder.UseSpaServer("/", entryPath);

            builder.UseFileServer(
                new FileServerOptions
                {
                    RequestPath = new PathString(string.Empty),
                    FileSystem = new PhysicalFileSystem(fileSystemPath),
                    EnableDirectoryBrowsing = true
                });

            return builder;
        }
    }
}