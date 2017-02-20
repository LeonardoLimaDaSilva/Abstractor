using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Abstractor.Owin.WebApi
{
    /// <summary>
    ///     Extends the IAppBuilder interface to provide the SPA server activator.
    /// </summary>
    internal static class SpaServerExtension
    {
        /// <summary>
        ///     Activates the static file server.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="rootPath">The entry point path for the SPA.</param>
        /// <param name="entryPath">The file system path used to locate resources.</param>
        /// <returns></returns>
        public static IAppBuilder UseSpaServer(this IAppBuilder builder, string rootPath, string entryPath)
        {
            var options = new SpaServerOptions
            {
                FileServerOptions = new FileServerOptions
                {
                    EnableDirectoryBrowsing = false,
                    FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootPath))
                },
                EntryPath = new PathString(entryPath)
            };

            builder.UseDefaultFiles(options.FileServerOptions.DefaultFilesOptions);

            return
                builder.Use(
                    new Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>(
                        next => new SpaServerMiddleware(next, options).Invoke));
        }
    }
}