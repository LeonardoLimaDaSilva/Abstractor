using System;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    public sealed class CommandPostAction : ICommandPostAction
    {
        public event Action Execute = () => { };

        public void Act()
        {
            Execute();
        }

        public void Reset()
        {
            Execute = () => { };
        }
    }
}
