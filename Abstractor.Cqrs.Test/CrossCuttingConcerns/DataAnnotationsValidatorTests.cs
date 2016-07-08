using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Test.Helpers;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class DataAnnotationsValidatorTests
    {
        public class FakeObject
        {
            [Required]
            public string Property { get; set; }
        }

        [Theory, AutoMoqData]
        public void Validate_Valid_ShouldPass(
            FakeObject fake,
            DataAnnotationsValidator validator)
        {
            // Act

            validator.Validate(fake);
        }

        [Theory, AutoMoqData]
        public void Validate_Invalid_ThrowsValidationException(
            FakeObject fake,
            DataAnnotationsValidator validator)
        {
            // Arrange

            fake.Property = null;

            // Act and assert

            Assert.Throws<ValidationException>(() => validator.Validate(fake));
        }
    }
}