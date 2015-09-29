using System;
using System.Reflection;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.SimpleInjector.CompositionRoot
{
    /// <summary>
    /// Configurações da raiz de composição do framework. Expões os assemblies necessários para compor as funcionalidades da aplicação.
    /// </summary>
    public sealed class CompositionRootSettings
    {
        /// <summary>
        /// Local onde se encontram os handlers <see cref="ICommandHandler{TCommand}"/> e <see cref="IQueryHandler{TQuery,TResult}"/>.
        /// </summary>
        public Assembly[] OperationAssemblies { get; set; }

        /// <summary>
        /// Local onde se encontra o handler <see cref="IEventHandler{TEvent}"/>.
        /// </summary>
        public Assembly[] EventAssemblies { get; set; }
        
        /// <summary>
        /// Local onde se encontra as implementações da camada de persistência.
        /// </summary>
        public Type[] PersistenceTypes { get; set; }
    }
}
