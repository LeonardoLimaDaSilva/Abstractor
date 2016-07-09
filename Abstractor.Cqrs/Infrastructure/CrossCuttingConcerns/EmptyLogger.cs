using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Default empty logger, used when there is no explicit implementation defined.
    /// </summary>
    public sealed class EmptyLogger : ILogger
    {
        /// <summary>
        ///     Does nothing.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        public void Log(string message)
        {
        }
    }
}