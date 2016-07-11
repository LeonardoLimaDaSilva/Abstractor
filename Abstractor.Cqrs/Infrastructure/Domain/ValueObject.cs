using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    /// A value object that can be compared through their properties.
    /// </summary>
    /// <typeparam name="T">Value object type.</typeparam>
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        /// <summary>
        ///     Attributes that will be compared to determine equality.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetAttributesToIncludeInEqualityCheck();

        /// <summary>
        ///     Determines whether the object has the same value as the other.
        /// </summary>
        /// <param name="other">Object to be compared to the current instance.</param>
        /// <returns>True if the objects have the same value.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as T);
        }

        /// <summary>
        ///     Determines whether the object has the same value as the other.
        /// </summary>
        /// <param name="other">Object to be compared to the current instance.</param>
        /// <returns>True if the objects have the same value.</returns>
        public bool Equals(T other)
        {
            return other != null && GetAttributesToIncludeInEqualityCheck()
                .SequenceEqual(other.GetAttributesToIncludeInEqualityCheck());
        }

        /// <summary>
        ///     Determines whether the object has the same value as the other.
        /// </summary>
        /// <param name="left">Left side of operation.</param>
        /// <param name="right">Right side of operation.</param>
        /// <returns>True if the objects have the same value.</returns>
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the object do not have the same value as the other.
        /// </summary>
        /// <param name="left">Left side of operation.</param>
        /// <param name="right">Right side of operation.</param>
        /// <returns>True if the objects do not have the same value.</returns>
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Returns the hash code of this instance.
        /// </summary>
        /// <returns>Instance hash code.</returns>
        public override int GetHashCode()
        {
            // algorithm that uses the multiplication of prime numbers to minimize the occurrence of conflicts 
            // between the hashcodes of the object properties

            return GetAttributesToIncludeInEqualityCheck()
                .Aggregate(17, (current, o) => current * 31 + (o?.GetHashCode() ?? 0));
        }
    }
}