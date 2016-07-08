using System.Collections.Generic;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Test.Helpers
{
    public class InMemoryLogger : ILogger
    {
        public List<string> Messages { get; set; }

        public InMemoryLogger()
        {
            Messages = new List<string>();
        }

        public void Log(string message)
        {
            Messages.Add(message);
        }
    }
}