using System.IO;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Test.Helpers;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class GenericFileTests
    {
        [Theory]
        [AutoMoqData]
        public void Dispose_ShouldDisposeTheInnerStream(string fileName)
        {
            // Arrange

            var stream = new MemoryStream();

            // Act and assert

            using (new GenericFile(fileName, stream))
            {
                stream.CanSeek.Should().Be.True();
                stream.CanRead.Should().Be.True();
            }

            stream.CanSeek.Should().Be.False();
            stream.CanRead.Should().Be.False();
        }

        [Theory]
        [AutoMoqData]
        public void Map_ShouldMapTheProperties(
            string fileName,
            Stream stream)
        {
            // Arrange and act

            using (var genericFile = new GenericFile(fileName, stream))
            {
                // Assert

                genericFile.FileName.Should().Be(fileName);
                genericFile.Stream.Should().Be(stream);
            }
        }
    }
}