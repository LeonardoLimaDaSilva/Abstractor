using System;
using Abstractor.Cqrs.Interfaces.Events;

namespace Abstractor.Cqrs.Infrastructure.Events
{
    /// <summary>
    ///     Base domain event that exposes the date and time the event ocurred.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        ///     The date and time the event ocurred expressed as Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        ///     Defines the current timestamp.
        /// </summary>
        protected DomainEvent()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}