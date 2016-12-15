using System.Collections.Generic;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Test.Helpers
{
    public class FakeLogger : ILogger
    {
        public List<string> Messages { get; set; }

        public FakeLogger()
        {
            Messages = new List<string>();
        }

        public void Log(string message)
        {
            Messages.Add(message);
        }
    }
}