using System;

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
        ///     Attribute constructor.
        /// </summary>
        /// <param name="name">The name of Azure container.</param>
        public AzureContainerAttribute(string name)
        {
            Name = name;
        }
    }
}