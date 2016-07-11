using System;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Represents an entity that can be identified by a primary key.
    /// </summary>
    /// <typeparam name="TId">Primary key type.</typeparam>
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        /// <summary>
        ///     Immutable primary key.
        /// </summary>
        public TId Id { get; }

        protected Entity(TId id)
        {
            Id = id;
        }

        /// <summary>
        ///     Determines whether this entity is equal to another.
        /// </summary>
        /// <param name="other">Entity to be compared to.</param>
        /// <returns>True if the other entity is not null, not transient, and have the same id of the current instance.</returns>
        public virtual bool Equals(Entity<TId> other)
        {
            // an instance will never be equal to null
            if (other == null) return false;

            // when the reference is the same, they are the same object
            if (ReferenceEquals(this, other)) return true;

            // returns false when an object is transient or ids are not equal
            if (IsTransient(this) || IsTransient(other) || !Equals(Id, other.Id)) return false;

            // if they are different instances but have the same id and one inherits the other
            // they are considered the same entity
            var otherType = other.GetType();
            var thisType = GetType();
            return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
        }

        /// <summary>
        ///     Returns the instance hash code based on id.
        /// </summary>
        /// <returns>Instance hash code.</returns>
        public override int GetHashCode()
        {
            return IsTransient(this) ? 0 : Id.GetHashCode();
        }

        /// <summary>
        ///     Determines whether this entity is equal to another.
        /// </summary>
        /// <param name="other">Entity to be compared to.</param>
        /// <returns>True if the other entity is not null, not transient, and have the same id of the current instance.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as Entity<TId>);
        }

        /// <summary>
        ///     Verifies if the entity's id has a default value.
        /// </summary>
        /// <param name="entity">Entity to be verified.</param>
        /// <returns></returns>
        private static bool IsTransient(Entity<TId> entity)
        {
            return Equals(entity.Id, default(TId));
        }
    }
}