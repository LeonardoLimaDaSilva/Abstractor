namespace Abstractor.Cqrs.SimpleInjector.AzureStorage.Interfaces
{
    /// <summary>
    /// Especifica os identificadores da tabela.
    /// </summary>
    public interface ITableKeys
    {
        string PartitionKey { get; }

        string RowKey { get; }
    }
}