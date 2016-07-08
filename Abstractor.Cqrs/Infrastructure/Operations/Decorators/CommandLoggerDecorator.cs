using System;
using System.Diagnostics;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;
using Abstractor.Cqrs.Interfaces.Operations;
using Newtonsoft.Json;

namespace Abstractor.Cqrs.Infrastructure.Operations.Decorators
{
    /// <summary>
    ///     Logs the execution of the command handler.
    /// </summary>
    /// <typeparam name="TCommand">Command to be handled.</typeparam>
    [DebuggerStepThrough]
    public sealed class CommandLoggerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<ICommandHandler<TCommand>> _handlerFactory;
        private readonly ILogger _logger;

        public CommandLoggerDecorator(
            Func<ICommandHandler<TCommand>> handlerFactory,
            ILogger logger)
        {
            _handlerFactory = handlerFactory;
            _logger = logger;
        }

        /// <summary>
        ///     Logs the execution of the command handler.
        /// </summary>
        /// <param name="command">Command to be handled.</param>
        public void Handle(TCommand command)
        {
            var sw = Stopwatch.StartNew();
            sw.Start();

            var handler = _handlerFactory();

            try
            {
                _logger.Log($"Executing command {handler.GetType().Name} with the parameters:");

                try
                {
                    _logger.Log(JsonConvert.SerializeObject(command, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    _logger.Log($"Could not serialize the parameters: {ex.Message}");
                }

                _handlerFactory().Handle(command);
            }
            catch (Exception ex)
            {
                _logger.Log("Exception caught: " + ex.Message);

                if (ex.InnerException != null)
                    _logger.Log("Inner exception caught: " + ex.InnerException.Message);

                throw;
            }
            finally
            {
                sw.Stop();

                _logger.Log($"Command \"{handler.GetType().Name}\" executed in {sw.Elapsed}.");
            }
        }
    }
}