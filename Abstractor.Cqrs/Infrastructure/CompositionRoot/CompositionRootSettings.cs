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
        ///     Application assemblies that contains the operations handlers.
        /// </summary>
        public IEnumerable<Assembly> OperationAssemblies { get; set; }

        /// <summary>
        ///     Implementation types from the persistence layer.
        /// </summary>
        public IEnumerable<Type> PersistenceTypes { get; set; }

        /// <summary>
        ///     Implementation types from the application layer.
        /// </summary>
        public IEnumerable<Type> ApplicationTypes { get; set; }
    }
}