using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    /// <summary>
    ///     Exposes the assemblies required for composing the application.
    /// </summary>
    public sealed class CompositionRootSettings
    {
        /// <summary>
        ///     Application assemblies that contains the concrete operations handlers.
        /// </summary>
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; }

        /// <summary>
        ///     Concrete implementation types from the application.
        /// </summary>
        public IEnumerable<Type> ApplicationTypes { get; set; }
    }
}