using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Owin.WebApi.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Owin.WebApi.Test
{
    public class GenericFileMediaTypeFormatterTests
    {
        public class FakeCommand
        {
            public DateTimeOffset DateProperty { get; set; }

            public GenericFile File { get; set; }

            public IEnumerable<GenericFile> Files { get; set; }

            public string StringProperty { get; set; }
        }

        [Fact]
        public void CanReadType_ShouldBeTrue()
        {
            var formatter = new GenericFileMediaTypeFormatter();
            formatter.CanReadType(typeof(object)).Should().Be.True();
        }

        [Fact]
        public void CanWriteType_ShouldBeFalse()
        {
            var formatter = new GenericFileMediaTypeFormatter();
            formatter.CanWriteType(typeof(object)).Should().Be.False();
        }

        [Theory]
        [AutoMoqData]
        public void ReadFromStreamAsync_BindMultipleFiles(IFormatterLogger formatterLogger)
        {
            var fileContent1 = new ByteArrayContent(new byte[100]);
            fileContent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "filename1.txt",
                Name = "files[0]"
            };

            var fileContent2 = new ByteArrayContent(new byte[200]);
            fileContent2.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "filename2.txt",
                Name = "files[1]"
            };

            var content = new MultipartFormDataContent
            {
                fileContent1,
                fileContent2
            };

            var formatter = new GenericFileMediaTypeFormatter();

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) formatter
                    .ReadFromStreamAsync(
                        typeof(FakeCommand),
                        ms,
                        content,
                        formatterLogger)
                    .Result;

                command.Files.ElementAt(0).FileName.Should().Be("filename1.txt");
                command.Files.ElementAt(0).Stream.Length.Should().Be(100);

                command.Files.ElementAt(1).FileName.Should().Be("filename2.txt");
                command.Files.ElementAt(1).Stream.Length.Should().Be(200);
            }
        }

        [Theory]
        [AutoMoqData]
        public async void ReadFromStreamAsync_BindSingleFile(IFormatterLogger formatterLogger)
        {
            var fileContent = new ByteArrayContent(new byte[100]);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "filename.txt",
                Name = "file"
            };

            var content = new MultipartFormDataContent {fileContent};

            var formatter = new GenericFileMediaTypeFormatter();

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) await formatter.ReadFromStreamAsync(
                    typeof(FakeCommand),
                    ms,
                    content,
                    formatterLogger);

                command.File.FileName.Should().Be("filename.txt");
                command.File.Stream.Length.Should().Be(100);
            }
        }

        [Theory]
        [AutoMoqData]
        public async void ReadFromStreamAsync_DateTimeProperty_ShouldKeepTimeZone(IFormatterLogger formatterLogger)
        {
            var formContent = new StringContent("2017-01-01T00:00:00-03:00");

            formContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "dateProperty"
            };

            var content = new MultipartFormDataContent {formContent};

            var formatter = new GenericFileMediaTypeFormatter();

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) await formatter.ReadFromStreamAsync(
                    typeof(FakeCommand),
                    ms,
                    content,
                    formatterLogger);

                command.DateProperty.UtcDateTime.Should().Be(new DateTime(2017, 1, 1, 3, 0, 0));
            }
        }

        [Theory]
        [AutoMoqData]
        public void ReadFromStreamAsync_InvalidContent_ShouldSetDefaultValue(
            IRequiredMemberSelector requiredMemberSelector,
            [Frozen] Mock<IFormatterLogger> formatterLogger)
        {
            var formContent = new StringContent("Invalid");

            formContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            var content = new MultipartFormDataContent {formContent};

            var formatter = new GenericFileMediaTypeFormatter
            {
                RequiredMemberSelector = requiredMemberSelector
            };

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) formatter
                    .ReadFromStreamAsync(
                        typeof(FakeCommand),
                        ms,
                        content,
                        formatterLogger.Object)
                    .Result;

                formatterLogger.Verify(l => l.LogError(string.Empty, It.IsAny<Exception>()), Times.Once);

                command.Should().Be.Null();
            }
        }

        [Theory]
        [AutoMoqData]
        public async void ReadFromStreamAsync_InvalidContentAndNullLogger_ThrowsException()
        {
            var formContent = new StringContent("Invalid");
            formContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            var content = new MultipartFormDataContent {formContent};

            var formatter = new GenericFileMediaTypeFormatter();

            using (var ms = new MemoryStream())
            {
                await Assert.ThrowsAsync<NullReferenceException>(async () => await formatter
                    .ReadFromStreamAsync(
                        typeof(FakeCommand),
                        ms,
                        content,
                        null));
            }
        }

        [Theory]
        [AutoMoqData]
        public void ReadFromStreamAsync_InvalidDateProperty_ShouldSetDefaultValue(
            IRequiredMemberSelector requiredMemberSelector)
        {
            var formContent = new StringContent("Invalid");

            formContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "dateProperty"
            };

            var content = new MultipartFormDataContent {formContent};

            var formatter = new GenericFileMediaTypeFormatter
            {
                RequiredMemberSelector = requiredMemberSelector
            };

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) formatter
                    .ReadFromStreamAsync(
                        typeof(FakeCommand),
                        ms,
                        content,
                        null)
                    .Result;

                command.DateProperty.Should().Be(DateTimeOffset.MinValue);
            }
        }

        [Theory]
        [AutoMoqData]
        public void ReadFromStreamAsync_InvalidDateProperty_ShouldSetDefaultValue(
            IRequiredMemberSelector requiredMemberSelector,
            [Frozen] Mock<IFormatterLogger> formatterLogger)
        {
            var formContent = new StringContent("Invalid");

            formContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "dateProperty"
            };

            var content = new MultipartFormDataContent {formContent};

            var formatter = new GenericFileMediaTypeFormatter
            {
                RequiredMemberSelector = requiredMemberSelector
            };

            using (var ms = new MemoryStream())
            {
                var command = (FakeCommand) formatter.ReadFromStreamAsync(
                                                         typeof(FakeCommand),
                                                         ms,
                                                         content,
                                                         formatterLogger.Object)
                                                     .Result;

                formatterLogger.Verify(l => l.LogError("DateProperty",
                        "The value 'Invalid' is not valid for DateProperty."),
                    Times.Once);

                command.DateProperty.Should().Be(DateTimeOffset.MinValue);
            }
        }

        [Fact]
        public void SupportedMediaTypes_ShouldSupportMultipartFormData()
        {
            var formatter = new GenericFileMediaTypeFormatter();
            formatter.SupportedMediaTypes.Any(s => s.MediaType == "multipart/form-data").Should().Be.True();
        }
    }
}