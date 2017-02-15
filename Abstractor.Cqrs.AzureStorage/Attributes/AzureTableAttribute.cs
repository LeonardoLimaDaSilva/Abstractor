using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the table name of Azure Blob Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureTableAttribute : Attribute
    {
        /// <summary>
        ///     The name of Azure table.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Attribute constructor.
        /// </summary>
        /// <param name="name">The name of Azure table.</param>
        public AzureTableAttribute(string name)
        {
            Name = name;
        }
    }
}