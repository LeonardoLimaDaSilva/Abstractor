using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Extends the IAppBuilder interface to provide the SPA activator.
    /// </summary>
    public static class SpaExtension
    {
        /// <summary>
        ///     Activates the static file server with generic configuration for SPAs.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entryPath">The entry point path for the SPA.</param>
        /// <param name="fileSystemPath">The file system path used to locate resources.</param>
        /// <returns></returns>
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