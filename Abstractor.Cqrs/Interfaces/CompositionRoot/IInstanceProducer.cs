using System;

namespace Abstractor.Cqrs.Interfaces.CompositionRoot
{
    /// <summary>
    ///     Abstracts the instance producer of the inversion of control container.
    /// </summary>
    public interface IInstanceProducer
    {
        /// <summary>
        ///     Sets the desired service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        ///     Gets the instance of the <see cref="ServiceType" />.
        /// </summary>
        /// <returns></returns>
        object GetInstance();
    }
}