namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Abstraction of the serializer used by the framework to log messages with complex types.
    /// </summary>
    public interface ILoggerSerializer
    {
        /// <summary>
        ///     Serializes an object.
        /// </summary>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Serialized string.</returns>
        string Serialize(object value);
    }
}