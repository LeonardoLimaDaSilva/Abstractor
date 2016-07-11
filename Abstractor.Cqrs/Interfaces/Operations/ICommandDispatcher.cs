using System.Threading.Tasks;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Dispatcher for a command handler.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        ///     Delegates the command parameters to the handler that implements <see cref="ICommandHandler{ICommand}" />.
        /// </summary>
        /// <param name="command">Command to be dispatched.</param>
        void Dispatch(ICommand command);

        /// <summary>
        ///     Delegates the command parameters asynchronously to the handler that implements <see cref="ICommandHandler{ICommand}" />.
        /// </summary>
        /// <param name="command">Command to be dispatched.</param>
        /// <returns>Asynchronous task.</returns>
        Task DispatchAsync(ICommand command);
    }
}