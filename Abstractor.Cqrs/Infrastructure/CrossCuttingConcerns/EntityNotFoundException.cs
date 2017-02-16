using System;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Exception to be thrown when an entity is not found.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// </summary>
        public EntityNotFoundException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">Exception message.</param>
        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }
}