using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Represents the current system date/time mechanism.
    /// </summary>
    public class SystemClock : IClock
    {
        /// <summary>
        ///     Gets a DateTime that represents the current date and time of the system, in local time format.
        /// </summary>
        /// <returns>Current date and time.</returns>
        public virtual DateTime Now()
        {
            return DateTime.Now;
        }
    }
}