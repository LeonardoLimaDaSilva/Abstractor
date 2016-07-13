using System;
using System.Linq;
using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Finds an attribute in a type.
    /// </summary>
    public class AttributeFinder : IAttributeFinder
    {
        /// <summary>
        ///     Verifies if a type is decorated with an attribute.
        /// </summary>
        /// <param name="typeToVerify">Type to be verified.</param>
        /// <param name="attribute">Attribute to be searched.</param>
        /// <returns>True if the type is decorated with attribute.</returns>
        public bool Decorates(Type typeToVerify, Type attribute)
        {
            return typeToVerify.CustomAttributes.Any(a => a.AttributeType == attribute);
        }
    }
}