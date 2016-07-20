using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the container name of Azure Blob Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureContainerAttribute : Attribute
    {
        public string Name { get; private set; }

        public AzureContainerAttribute(string name)
        {
            Name = name;
        }
    }
}