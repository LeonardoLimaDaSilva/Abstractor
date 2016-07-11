using System;
using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Interfaces.Persistence;
using SharpTestsEx;
using static System.Threading.Thread;

namespace Abstractor.Test.Helpers
{
    public class FakeUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly List<Tuple<int, bool>> _commits;

        public FakeUnitOfWork()
        {
            _commits = new List<Tuple<int, bool>>();
        }

        public void Dispose()
        {
            _commits.RemoveAll(t => t.Item1 == CurrentThread.ManagedThreadId);
        }

        public void Commit()
        {
            _commits.Add(new Tuple<int, bool>(CurrentThread.ManagedThreadId, true));
        }

        public void SetUp()
        {
            Dispose();
        }

        public void CommittedShouldBe(bool expected)
        {
            _commits.SingleOrDefault(t => t.Item1 == CurrentThread.ManagedThreadId)
                ?.Item2
                    .Should()
                    .Be(expected);
        }
    }
}