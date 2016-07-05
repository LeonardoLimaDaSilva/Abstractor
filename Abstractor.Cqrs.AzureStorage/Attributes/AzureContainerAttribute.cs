using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Especifica o nome do container do Azure Blob Storage que a classe está mapeada.
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