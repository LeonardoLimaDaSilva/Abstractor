using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Test.Helpers;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class DataAnnotationsValidatorTests
    {
        [Theory, AutoMoqData]
        internal void Validate_Valid_ShouldPass(
            FakeObject fake,
            DataAnnotationsValidator validator)
        {
            validator.Validate(fake);
        }

        [Theory, AutoMoqData]
        internal void Validate_Invalid_ThrowsValidationException(
            FakeObject fake,
            DataAnnotationsValidator validator)
        {
            // Arrange

            fake.Property = null;

            // Act and assert

            Assert.Throws<ValidationException>(() => validator.Validate(fake));
        }

        internal class FakeObject
        {
            [Required]
            public string Property { get; set; }
        }
    }
}