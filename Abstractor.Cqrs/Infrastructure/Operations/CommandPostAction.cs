using System;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    /// <summary>
    ///     Executa uma ação após a finalização de um comando.
    /// </summary>
    public sealed class CommandPostAction : ICommandPostAction
    {
        /// <summary>
        ///     Ação que será executada após a finalização de um comando.
        /// </summary>
        public event Action Execute = () => { };

        /// <summary>
        ///     Executa a ação registrada em <see cref="Execute" />.
        /// </summary>
        public void Act()
        {
            Execute();
        }

        /// <summary>
        ///     Remove qualquer ação registrada em <see cref="Execute" />.
        /// </summary>
        public void Reset()
        {
            Execute = () => { };
        }
    }
}