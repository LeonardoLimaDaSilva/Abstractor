using System;

namespace Abstractor.Cqrs.AzureStorage.Attributes
{
    /// <summary>
    ///     Especifica o nome da tabela do Azure Table Storage que a classe está mapeada.
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