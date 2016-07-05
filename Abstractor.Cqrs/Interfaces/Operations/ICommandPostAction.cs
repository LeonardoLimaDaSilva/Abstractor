using System;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Executa uma ação após a finalização de um comando.
    /// </summary>
    public interface ICommandPostAction
    {
        /// <summary>
        ///     Ação que será executada após a finalização de um comando.
        /// </summary>
        event Action Execute;

        /// <summary>
        ///     Executa a ação registrada em <see cref="Execute" />.
        /// </summary>
        void Act();

        /// <summary>
        ///     Remove qualquer ação registrada em <see cref="Execute" />.
        /// </summary>
        void Reset();
    }
}