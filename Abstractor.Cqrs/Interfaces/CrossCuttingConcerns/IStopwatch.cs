using System;

namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Abstraction of a stopwatch.
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        ///     Gets the elapsed interval.
        /// </summary>
        /// <returns>Elapsed interval.</returns>
        TimeSpan GetElapsed();

        /// <summary>
        ///     Starts measuring elapsed time for an interval.
        /// </summary>
        void Start();

        /// <summary>
        ///     Gets the total elapsed time measured by the current instance.
        /// </summary>
        void Stop();
    }
}