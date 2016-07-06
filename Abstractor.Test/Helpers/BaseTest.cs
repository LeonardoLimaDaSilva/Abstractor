using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Test.Helpers
{
    public class BaseTest
    {
        public IQueryDispatcher QueryDispatcher { get; set; }

        public ICommandDispatcher CommandDispatcher { get; set; }

        public BaseTest()
        {
            CommandDispatcher = CompositionRoot.GetContainer().GetInstance<ICommandDispatcher>();
            QueryDispatcher = CompositionRoot.GetContainer().GetInstance<IQueryDispatcher>();
        }
    }
}