using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Logger vazio utilizado quando nenhuma outra implementação for explicitamente definida.
    /// </summary>
    public sealed class EmptyLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }
}