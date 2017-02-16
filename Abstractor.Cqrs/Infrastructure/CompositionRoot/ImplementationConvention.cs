namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    /// <summary>
    ///     Conventions used to discover the application implementation types.
    /// </summary>
    public enum ImplementationConvention
    {
        /// <summary>
        ///     Finds all types that the name starts with the correspondent string.
        /// </summary>
        NameStartsWith,

        /// <summary>
        ///     Finds all types that the name ends with the correspondent string.
        /// </summary>
        NameEndsWith,

        /// <summary>
        ///     Finds all types that the name contains the correspondent string.
        /// </summary>
        NameContains,

        /// <summary>
        ///     Finds all types that the containing namespace starts with the correspondent string.
        /// </summary>
        NamespaceStartsWith,

        /// <summary>
        ///     Finds all types that the containing namespace ends with the correspondent string.
        /// </summary>
        NamespaceEndsWith,

        /// <summary>
        ///     Finds all types that the containing namespace contains with the correspondent string.
        /// </summary>
        NamespaceContains
    }
}