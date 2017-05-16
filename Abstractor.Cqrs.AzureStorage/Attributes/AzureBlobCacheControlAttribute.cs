using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the cache control value stored for the blob.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureBlobCacheControlAttribute : Attribute
    {
        /// <summary>
        ///     Cache control property value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        ///     Attribute constructor.
        /// </summary>
        /// <param name="value"></param>
        public AzureBlobCacheControlAttribute(string value)
        {
            Value = value;
        }
    }
}