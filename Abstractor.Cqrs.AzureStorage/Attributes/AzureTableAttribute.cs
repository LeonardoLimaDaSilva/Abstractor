using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the table name of Azure Blob Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureTableAttribute : Attribute
    {
        public string Name { get; private set; }

        public AzureTableAttribute(string name)
        {
            Name = name;
        }
    }
}