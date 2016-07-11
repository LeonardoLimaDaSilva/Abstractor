using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Interfaces.Persistence;

namespace Abstractor.Test.Helpers
{
    public abstract class BaseTest
    {
        public IQueryDispatcher QueryDispatcher { get; set; }

        public ICommandDispatcher CommandDispatcher { get; set; }

        public FakeLogger Logger { get; set; }

        public FakeUnitOfWork UnitOfWork { get; set; }

        protected BaseTest()
        {
            CommandDispatcher = CompositionRoot.GetContainer().GetInstance<ICommandDispatcher>();
            QueryDispatcher = CompositionRoot.GetContainer().GetInstance<IQueryDispatcher>();
            Logger = (FakeLogger) CompositionRoot.GetContainer().GetInstance<ILogger>();
            UnitOfWork = (FakeUnitOfWork) CompositionRoot.GetContainer().GetInstance<IUnitOfWork>();
        }
    }
}