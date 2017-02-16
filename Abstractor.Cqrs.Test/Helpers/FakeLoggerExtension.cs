using System.Linq;
using SharpTestsEx;

namespace Abstractor.Cqrs.Test.Helpers
{
    public static class FakeLoggerExtension
    {
        public static void ShouldBeCalled(this FakeLogger logger)
        {
            logger.Messages.Any().Should().Be.True();
        }

        public static void ShouldNeverBeCalled(this FakeLogger logger)
        {
            logger.Messages.Any().Should().Be.False();
        }

        public static void VerifyMessages(this FakeLogger logger, params string[] messages)
        {
            for (var i = 0; i < messages.Length; i++)
                logger.Messages[i].Should().Be(messages[i]);
        }
    }
}