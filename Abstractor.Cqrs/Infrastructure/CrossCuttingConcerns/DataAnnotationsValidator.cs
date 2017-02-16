using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Validates an object using the default DataAnnotations <see cref="Validator" />.
    /// </summary>
    public sealed class DataAnnotationsValidator : IValidator
    {
        /// <summary>
        ///     Throws an exception if the instance of an object is invalid.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        public void Validate(object instance)
        {
            var context = new ValidationContext(instance, null, null);
            Validator.ValidateObject(instance, context, true);
        }
    }
}