using System;
using System.Diagnostics;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Provides common methods to check the integrity of pre-conditions used to avoid errors during execution.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if the argument is null.
        /// </summary>
        /// <param name="value">Object value.</param>
        /// <param name="argument">Argument name.</param>
        /// <param name="message">Optional exception message.</param>
        [DebuggerHidden]
        public static void ArgumentIsNotNull(object value, string argument, string message = null)
        {
            if (value != null) return;

            if (message == null) throw new ArgumentNullException(argument);

            throw new ArgumentNullException(argument, message);
        }

        /// <summary>
        ///     Throws an <see cref="EntityNotFoundException" /> if an entity is null.
        /// </summary>
        /// <param name="value">Entity object.</param>
        [DebuggerHidden]
        public static void EntityIsNotNull(object value)
        {
            if (value != null) return;

            throw new EntityNotFoundException();
        }

        /// <summary>
        ///     Throws an <see cref="EntityNotFoundException" /> if an entity is null, stating the entity type and the primary key.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="value">Entity object.</param>
        /// <param name="primaryKey">Entity's primary key.</param>
        [DebuggerHidden]
        public static void EntityIsNotNull<T>(object value, object primaryKey)
        {
            if (value != null) return;

            throw new EntityNotFoundException(
                $"A entidade do tipo '{typeof (T).Name}' não foi encontrada para a chave primária '{primaryKey}'.");
        }
    }
}