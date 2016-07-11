using System;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Test.Helpers
{
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