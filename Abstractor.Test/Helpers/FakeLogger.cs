using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using SharpTestsEx;

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
            _messages.RemoveAll(t => t.Item1 == Thread.CurrentThread.ManagedThreadId);
        }

        public void Log(string message)
        {
            _messages.Add(new Tuple<int, string>(Thread.CurrentThread.ManagedThreadId, message));
        }

        public void SetUp()
        {
            Dispose();
        }

        public void MessagesShouldBe(params string[] expected)
        {
            var messagesPerThread = _messages.Where(t => t.Item1 == Thread.CurrentThread.ManagedThreadId).ToList();

            for (var i = 0; i < expected.Length; i++)
                messagesPerThread[i].Item2.Should().Be(expected[i]);
        }
    }
}