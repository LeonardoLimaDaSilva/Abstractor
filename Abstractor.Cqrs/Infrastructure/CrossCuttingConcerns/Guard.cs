using System;
using System.Diagnostics;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Disponibiliza métodos comuns para verificar a integridade de pré-condições utilizadas
    ///     para evitar erros durante a execução.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        ///     Lança uma exceção do tipo <see cref="ArgumentNullException" /> caso o argumento seja nulo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="argument"></param>
        /// <param name="message">Mensagem da exceção.</param>
        [DebuggerHidden]
        public static void ArgumentIsNotNull(object value, string argument, string message = null)
        {
            if (value != null) return;

            if (message == null) throw new ArgumentNullException(argument);

            throw new ArgumentNullException(argument, message);
        }

        /// <summary>
        ///     Lança uma exceção do tipo <see cref="EntityNotFoundException" /> caso a entidade seja nula.
        /// </summary>
        /// <param name="value">Objeto da entidade.</param>
        [DebuggerHidden]
        public static void EntityIsNotNull(object value)
        {
            if (value != null) return;

            throw new EntityNotFoundException();
        }

        /// <summary>
        ///     Lança uma exceção do tipo <see cref="EntityNotFoundException" /> caso a entidade seja nula, informando o tipo da
        ///     entidade e a chave primária utilizada.
        /// </summary>
        /// <typeparam name="T">Tipo da entidade.</typeparam>
        /// <param name="value">Objeto da entidade.</param>
        /// <param name="primaryKey">Chave primária.</param>
        [DebuggerHidden]
        public static void EntityIsNotNull<T>(object value, object primaryKey)
        {
            if (value != null) return;

            throw new EntityNotFoundException(
                $"A entidade do tipo '{typeof (T).Name}' não foi encontrada para a chave primária '{primaryKey}'.");
        }
    }
}