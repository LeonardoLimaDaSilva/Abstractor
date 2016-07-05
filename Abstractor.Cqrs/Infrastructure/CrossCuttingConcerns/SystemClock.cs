using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Representa o mecanismo de data e hora do sistema.
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        ///     Obtém um DateTime que representa a data e hora corrente do sistema, no formato de hora local.
        /// </summary>
        /// <returns>Data e hora corrente no formato de hora local.</returns>
        public virtual DateTime Now()
        {
            return DateTime.Now;
        }
    }
}