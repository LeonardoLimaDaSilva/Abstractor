using System;

namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    /// Representa um mecanismo de data e hora.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Obtém um DateTime que representa data e hora corrente.
        /// </summary>
        /// <returns>Data e hora corrente.</returns>
        DateTime Now();
    }
}