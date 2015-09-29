namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    /// Registrador de mensagens de log.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Registra a mensagem de log.
        /// </summary>
        /// <param name="message">Mensagem a ser registrada.</param>
        void Log(string message);
    }
}
