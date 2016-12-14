namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    /// <summary>
    ///     Global settings for the framework.
    /// </summary>
    public sealed class GlobalSettings
    {
        /// <summary>
        ///     Enable the transactional behavior globally.
        /// </summary>
        public bool EnableTransactions { get; set; }

        /// <summary>
        ///     Enable the logging behavior globally.
        /// </summary>
        public bool EnableLogging { get; set; }
    }
}