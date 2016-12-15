using System.Diagnostics.CodeAnalysis;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class FakeLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }
}