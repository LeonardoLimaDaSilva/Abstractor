namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    /// Define o comando como um comando de criação de um item de agregação.
    /// </summary>
    /// <typeparam name="TAggregate">A agregação que será criada.</typeparam>
    public interface ICreateAggregateCommand<TAggregate> : ICommand where TAggregate : class
    {
        /// <summary>
        /// Instância da agregação criada.
        /// </summary>
        TAggregate CreatedAggregate { get; set; }
    }
}
