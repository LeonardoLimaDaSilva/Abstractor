using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the queue name of Azure Queue Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureQueueAttribute : Attribute
    {
        public string Name { get; private set; }

        public AzureQueueAttribute(string name)
        {
            Name = name;
        }
    }
}