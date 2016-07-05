using System.ComponentModel.DataAnnotations;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Valida um objeto usando DataAnnotations.
    /// </summary>
    public sealed class DataAnnotationsValidator : IValidator
    {
        public void Validate(object instance)
        {
            var context = new ValidationContext(instance, null, null);
            Validator.ValidateObject(instance, context, true);
        }
    }
}