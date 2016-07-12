using Abstractor.Cqrs.Infrastructure.Persistence;
using Xunit;

namespace Abstractor.Cqrs.Test.Persistence
{
    public class EmptyUnitOfWorkTests
    {
        /// <summary>
        ///     Just to please the code coverage tool.
        /// </summary>
        [Fact]
        public void Commit_DoNothing()
        {
            new EmptyUnitOfWork().Commit();
        }
    }
}