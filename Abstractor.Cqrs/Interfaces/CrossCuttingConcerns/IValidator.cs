namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Abstracts a validator of objects.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        ///     Throws an exception if the instance of an object is invalid.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        void Validate(object instance);
    }
}