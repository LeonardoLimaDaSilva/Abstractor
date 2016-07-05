using System;
using System.Collections.Generic;
using System.Reflection;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    /// <summary>
    ///     Configurações da raiz de composição do framework. Expões os assemblies necessários para compor as funcionalidades
    ///     da aplicação.
    /// </summary>
    public sealed class CompositionRootSettings
    {
        /// <summary>
        ///     Local onde se encontram os handlers que implementam <see cref="ICommandHandler{TCommand}" /> e
        ///     <see cref="IQueryHandler{TQuery,TResult}" />.
        /// </summary>
        public IEnumerable<Assembly> OperationAssemblies { get; set; }

        /// <summary>
        ///     Local onde se encontra os handlers que implementam <see cref="IEventHandler{TEvent}" />.
        /// </summary>
        public IEnumerable<Assembly> EventAssemblies { get; set; }

        /// <summary>
        ///     Tipos das implementações da camada de persistência.
        /// </summary>
        public IEnumerable<Type> PersistenceTypes { get; set; }

        /// <summary>
        ///     Tipos das implementações da camada de aplicação.
        /// </summary>
        public IEnumerable<Type> ApplicationTypes { get; set; }
    }
}