using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Default implementation that uses the <see cref="Stopwatch" />.
    /// </summary>
    public class DefaultStopwatch : IStopwatch
    {
        private Stopwatch _stopwatch;

        /// <summary>
        ///     Gets the elapsed interval.
        /// </summary>
        /// <returns>Elapsed interval.</returns>
        public TimeSpan GetElapsed()
        {
            return _stopwatch.Elapsed;
        }

        /// <summary>
        ///     Starts measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
        }

        /// <summary>
        ///     Gets the total elapsed time measured by the current instance.
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
        }
    }
}