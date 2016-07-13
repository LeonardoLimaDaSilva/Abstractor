using System;

namespace Abstractor.Cqrs.Interfaces.CrossCuttingConcerns
{
    /// <summary>
    ///     Finds an attribute in a type.
    /// </summary>
    public interface IAttributeFinder
    {
        /// <summary>
        ///     Verifies if a type is decorated with an attribute.
        /// </summary>
        /// <param name="typeToVerify">Type to be verified.</param>
        /// <param name="attribute">Attribute to be searched.</param>
        /// <returns>True if the type is decorated with attribute.</returns>
        bool Decorates(Type typeToVerify, Type attribute);
    }
}