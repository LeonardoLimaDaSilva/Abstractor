using System.IO;
using Abstractor.Cqrs.Infrastructure.Domain;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Domain
{
    public class GenericFileExtensionTests
    {
        [Fact]
        public void IsValid_IsNull_ReturnsFalse()
        {
            // Arrange, act and assert

            ((GenericFile) null).IsValid().Should().Be.False();
        }

        [Fact]
        public void IsValid_FileNameIsEmpty_ReturnsFalse()
        {
            // Arrange

            var memoryStream = new MemoryStream(new byte[] {0});

            using (var genericFile = new GenericFile(string.Empty, memoryStream))
            {
                // Act and assert

                genericFile.IsValid().Should().Be.False();
            }
        }

        [Fact]
        public void IsValid_MemoryStreamIsEmpty_ReturnsFalse()
        {
            // Arrange

            var memoryStream = new MemoryStream();

            using (var genericFile = new GenericFile("fileName", memoryStream))
            {
                // Act and assert

                genericFile.IsValid().Should().Be.False();
            }
        }

        [Fact]
        public void IsValid_ValidInstance_ReturnsTrue()
        {
            // Arrange

            var memoryStream = new MemoryStream(new byte[] { 0 });

            using (var genericFile = new GenericFile("fileName", memoryStream))
            {
                // Act and assert

                genericFile.IsValid().Should().Be.True();
            }
        }
    }
}