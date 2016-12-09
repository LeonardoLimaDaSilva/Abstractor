using System;
using System.Diagnostics.CodeAnalysis;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Test.Helpers
{
    [ExcludeFromCodeCoverage]
    public class FakeStopwatch : IStopwatch
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public TimeSpan GetElapsed()
        {
            return TimeSpan.Zero;
        }
    }
}