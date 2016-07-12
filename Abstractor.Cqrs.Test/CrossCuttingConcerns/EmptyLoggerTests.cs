using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class EmptyLoggerTests
    {
        /// <summary>
        ///     Just to please the code coverage tool.
        /// </summary>
        [Fact]
        public void Log_DoNothing()
        {
            new EmptyLogger().Log(string.Empty);
        }
    }
}