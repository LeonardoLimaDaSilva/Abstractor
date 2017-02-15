using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Defines the queue name of Azure Queue Storage that the class is mapped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AzureQueueAttribute : Attribute
    {
        /// <summary>
        ///     The name of Azure queue.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Attribute constructor.
        /// </summary>
        /// <param name="name">The name of Azure queue.</param>
        public AzureQueueAttribute(string name)
        {
            Name = name;
        }
    }
}