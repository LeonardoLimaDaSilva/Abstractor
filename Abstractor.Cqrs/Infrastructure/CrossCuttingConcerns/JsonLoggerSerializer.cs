using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Serializer that uses <see cref="JsonConvert" />.
    /// </summary>
    public class JsonLoggerSerializer : ILoggerSerializer
    {
        /// <summary>
        ///     Serializes an object into an indented json string.
        /// </summary>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Serialized string.</returns>
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}