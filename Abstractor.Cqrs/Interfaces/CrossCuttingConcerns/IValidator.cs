using System;
using System.ComponentModel.DataAnnotations;

namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Validador de instâncias de objetos.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        ///     Valida uma instância.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <exception cref="ArgumentNullException">Se a instância for nula.</exception>
        /// <exception cref="ValidationException">Se a instância for inválida.</exception>
        void Validate(object instance);
    }
}