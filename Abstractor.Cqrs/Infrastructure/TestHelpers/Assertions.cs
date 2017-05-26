using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Abstractor.Cqrs.Infrastructure.CompositionRoot.Extensions;
using Abstractor.Cqrs.Infrastructure.Operations;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;

namespace Abstractor.Cqrs.Infrastructure.TestHelpers
{
    /// <summary>
    ///     Provides testing helper methods to verify the consistence of application artifacts.
    /// </summary>
    public static class Assertions
    {
        /// <summary>
        ///     Analyses whether there are any command handler that do not adheres to the recommended pattern of being an internal
        ///     and sealed class.
        /// </summary>
        /// <param name="commandHandlersAssemblies">Assemblies that contains the command handlers.</param>
        public static void ThrowsIfAnyCommandHandlerIsNotInternalSealed(params Assembly[] commandHandlersAssemblies)
        {
            var handlers = FindCommandHandlers(commandHandlersAssemblies);

            InternalSealedAssertion(handlers, "The following command handlers are not internal sealed");
        }

        /// <summary>
        ///     Analyses whether there are any domain event handler that do not adheres to the recommended pattern of being an
        ///     internal and sealed class.
        /// </summary>
        /// <param name="eventHandlersAssemblies">Assemblies that contains the domain event handlers.</param>
        public static void ThrowsIfAnyDomainEventHandlerIsNotInternalSealed(params Assembly[] eventHandlersAssemblies)
        {
            var handlers = FindDomainEventHandlers(eventHandlersAssemblies);

            InternalSealedAssertion(handlers, "The following domain event handlers are not internal sealed");
        }

        /// <summary>
        ///     Analyses whether there are any query handler that do not adheres to the recommended pattern of being an internal
        ///     and sealed class.
        /// </summary>
        /// <param name="queryHandlersAssemblies"></param>
        public static void ThrowsIfAnyQueryHandlerIsNotInternalSealed(params Assembly[] queryHandlersAssemblies)
        {
            var handlers = FindQueryHandlers(queryHandlersAssemblies);

            InternalSealedAssertion(handlers, "The following query handlers are not internal sealed");
        }

        /// <summary>
        ///     Analyses whether there are any command that is not being handled by a command handler.
        /// </summary>
        /// <param name="commandsAssembly">Assembly that contains the commands.</param>
        /// <param name="commandHandlersAssemblies">Assemblies that contains the command handlers.</param>
        public static void ThrowsIfHasUnhandledCommands(
            Assembly commandsAssembly,
            params Assembly[] commandHandlersAssemblies)
        {
            var handlers = FindCommandHandlers(commandHandlersAssemblies);

            var sb = new StringBuilder();

            var commands = commandsAssembly.DefinedTypes.Where(t => typeof(ICommand).IsAssignableFrom(t));

            foreach (var c in commands)
                if (!handlers.Any(h => typeof(CommandHandler<>).MakeGenericType(c).IsAssignableFrom(h.BaseType)))
                    sb.AppendLine(c.Name);

            Assert(sb, "No command handler was found for the following commands");
        }

        /// <summary>
        ///     Analyses whether there are any domain event that is not being handled by an domain event handler.
        /// </summary>
        /// <param name="eventsAssembly">Assembly that contains the events.</param>
        /// <param name="eventHandlersAssemblies">Assemblies that contains the domain event handlers.</param>
        public static void ThrowsIfHasUnhandledDomainEvents(
            Assembly eventsAssembly,
            params Assembly[] eventHandlersAssemblies)
        {
            var handlers = FindDomainEventHandlers(eventHandlersAssemblies);

            var sb = new StringBuilder();

            var events = eventsAssembly.DefinedTypes.Where(t => typeof(IDomainEvent).IsAssignableFrom(t));

            foreach (var e in events)
                if (!handlers.Any(h => typeof(IDomainEventHandler<>).MakeGenericType(e).IsAssignableFrom(h)))
                    sb.AppendLine(e.Name);

            Assert(sb, "No event handler was found for the following domain events");
        }

        /// <summary>
        ///     Analyses whether there are any query that is not being handled by a query handler.
        /// </summary>
        /// <param name="queriesAssembly">Assembly that contains the queries.</param>
        /// <param name="queryHandlersAssemblies">Assemblies that contains the query handlers.</param>
        public static void ThrowsIfHasUnhandledQueries(
            Assembly queriesAssembly,
            params Assembly[] queryHandlersAssemblies)
        {
            var handlers = FindQueryHandlers(queryHandlersAssemblies);

            var sb = new StringBuilder();

            var queries = queriesAssembly.DefinedTypes.Where(t => t.ImplementsOpenGenericInterface(typeof(IQuery<>)));

            var count = 0;

            foreach (var q in queries)
            {
                count += handlers
                    .SelectMany(h => h.GetInterfaces())
                    .Count(hi => hi.GetGenericArguments().FirstOrDefault(ga => ga == q) != null);

                if (count == 0) sb.AppendLine(q.Name);

                count = 0;
            }

            Assert(sb, "No query handler was found for the following queries");
        }

        private static void Assert(StringBuilder sb, string message)
        {
            if (sb.Length <= 0) return;
            throw new Exception($"{message}: {Environment.NewLine}{sb}");
        }

        private static List<TypeInfo> FindCommandHandlers(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.BaseType.ImplementsOpenGenericInterface(typeof(CommandHandler<>)))
                .ToList();
        }

        private static List<TypeInfo> FindDomainEventHandlers(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.ImplementsOpenGenericInterface(typeof(IDomainEventHandler<>)))
                .ToList();
        }

        private static List<TypeInfo> FindQueryHandlers(IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t
                    .GetInterfaces()
                    .Any(i =>
                        i.ImplementsOpenGenericInterface(typeof(IQueryAsyncHandler<,>)) ||
                        i.ImplementsOpenGenericInterface(typeof(IQueryHandler<,>)
                        )))
                .ToList();
        }

        private static void InternalSealedAssertion(IEnumerable<Type> handlers, string message)
        {
            var sb = new StringBuilder();

            foreach (var h in handlers.Where(h => !h.IsSealed || h.IsVisible))
                sb.AppendLine(h.Name);

            Assert(sb, message);
        }
    }
}