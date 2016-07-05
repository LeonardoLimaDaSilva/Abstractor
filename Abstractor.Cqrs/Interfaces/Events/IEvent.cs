namespace Abstractor.Cqrs.Interfaces.Events
{
    /// <summary>
    ///     Especifica que a classe pode ser notificada pela implementação do <see cref="IEventHandler{TEvent}" />.
    /// </summary>
    /// <remarks>
    ///     O framework irá disparar automaticamente os manipuladores de eventos <see cref="IEventHandler{TEvent}" />,
    ///     usando os triggers <see cref="IEventTrigger{TEvent}" /> nos comandos que implementarem a interface
    ///     <see cref="IEvent" />.
    /// </remarks>
    /// <example>
    ///     O código mostra a implementação de um <see cref="IEvent" /> em um comando.
    ///     <code lang="cs">
    ///     <![CDATA[
    ///         public class ConfirmOrder : ICommand, IEvent
    ///         {
    ///             public int OrderId { get; set; }
    ///         }
    /// 
    ///         public class OnOrderConfirmed : IEventHandler<ConfirmOrder> 
    ///         {    
    ///             public Task Handle(ConfirmOrder @event) 
    ///             {
    ///                 //Executa alguma ação
    ///             }
    ///         }
    ///     ]]>
    ///     </code>
    /// </example>
    public interface IEvent
    {
    }
}