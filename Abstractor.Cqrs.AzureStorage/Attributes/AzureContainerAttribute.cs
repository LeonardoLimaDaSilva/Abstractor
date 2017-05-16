using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the container name of Azure Blob Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureContainerAttribute : Attribute
    {
        /// <summary>
        ///     The name of Azure container.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Specifies the level of public access allowed in this container.
        /// </summary>
        public BlobContainerPublicAccessType PublicAccessType { get; }

        /// <summary>
        ///     Attribute constructor.
        /// </summary>
        /// <param name="name">The name of Azure container.</param>
        /// <param name="publicAccessType">Specifies the level of public access allowed in this container.</param>
        public AzureContainerAttribute(
            string name,
            BlobContainerPublicAccessType publicAccessType = BlobContainerPublicAccessType.Off)
        {
            Name = name;
            PublicAccessType = publicAccessType;
        }
    }
}