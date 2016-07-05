using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Especifica o nome da fila do Azure Queue Storage que a classe está mapeada.
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