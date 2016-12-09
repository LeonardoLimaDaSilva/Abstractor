using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Abstractor.Cqrs.Interfaces.Persistence;
using SharpTestsEx;

namespace Abstractor.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class FakeUnitOfWork : IUnitOfWork
    {
        private readonly List<Tuple<int, bool>> _commits;

        public FakeUnitOfWork()
        {
            _commits = new List<Tuple<int, bool>>();
        }

        public void Commit()
        {
            _commits.Add(new Tuple<int, bool>(Thread.CurrentThread.ManagedThreadId, true));
        }

        public void Clear()
        {
            _commits.Clear();
        }

        public void SetUp()
        {
            _commits.RemoveAll(t => t.Item1 == Thread.CurrentThread.ManagedThreadId);
        }
        
        public void CommittedShouldBe(bool expected)
        {
            _commits.SingleOrDefault(t => t.Item1 == Thread.CurrentThread.ManagedThreadId)
                ?.Item2
                    .Should()
                    .Be(expected);
        }
    }
}