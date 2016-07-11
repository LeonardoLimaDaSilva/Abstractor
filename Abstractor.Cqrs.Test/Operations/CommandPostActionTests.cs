using Abstractor.Cqrs.Infrastructure.Operations;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations
{
    public class CommandPostActionTests
    {
        [Fact]
        public void Act_ShouldCallExecuteEvent()
        {
            // Arrange

            var executed = false;
            var action = new CommandPostAction();

            action.Execute += () => { executed = true; };

            // Act

            action.Act();

            // Assert

            executed.Should().Be.True();
        }

        [Fact]
        public void Reset_ShouldNotCallTheSettedUpAction()
        {
            // Arrange

            var executed = false;
            var action = new CommandPostAction();

            action.Execute += () => { executed = true; };

            // Act

            action.Reset();
            action.Act();

            // Assert

            executed.Should().Be.False();
        }
    }
}