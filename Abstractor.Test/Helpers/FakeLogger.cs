using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using SharpTestsEx;
using static System.Threading.Thread;

namespace Abstractor.Test.Helpers
{
    public class FakeLogger : ILogger, IDisposable
    {
        private readonly List<Tuple<int, string>> _messages;

        public FakeLogger()
        {
            _messages = new List<Tuple<int, string>>();
        }

        public void Dispose()
        {
            _messages.RemoveAll(t => t.Item1 == CurrentThread.ManagedThreadId);
        }

        public void Log(string message)
        {
            _messages.Add(new Tuple<int, string>(CurrentThread.ManagedThreadId, message));
        }

        public void MessagesShouldBe(params string[] expected)
        {
            var messagesPerThread = _messages.Where(t => t.Item1 == CurrentThread.ManagedThreadId).ToList();

            for (var i = 0; i < expected.Length; i++)
                messagesPerThread[i].Item2.Should().Be(expected[i]);
        }

        public IEnumerable<string> GetMessages()
        {
            return _messages.Where(t => t.Item1 == CurrentThread.ManagedThreadId)
                .Select(m => m.Item2)
                .ToList();
        }
    }
}