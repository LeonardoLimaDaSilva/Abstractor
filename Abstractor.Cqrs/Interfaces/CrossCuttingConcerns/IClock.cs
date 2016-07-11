using System;

namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Represents a date/time mechanism.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        ///     Gets a DateTime that represents the current date and time .
        /// </summary>
        /// <returns>Current date and time.</returns>
        DateTime Now();
    }
}