namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Abstraction of the logger used by the framework.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="message">Message to be logged.</param>
        void Log(string message);
    }
}