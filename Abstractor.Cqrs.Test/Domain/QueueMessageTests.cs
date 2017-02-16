using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class QueueMessageTests
    {
        /// <summary>
        ///     Just to please the code coverage tool.
        /// </summary>
        [Theory]
        [AutoMoqData]
        public void Mapping_ShouldSetData(string data)
        {
            var message = new QueueMessage
            {
                Object = data
            };

            message.Object.Should().Be(data);
        }
    }
}