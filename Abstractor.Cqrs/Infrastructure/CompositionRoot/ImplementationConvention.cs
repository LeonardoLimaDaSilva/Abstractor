namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    /// <summary>
    ///     Conventions used to discover the application implementation types.
    /// </summary>
    public enum ImplementationConvention
    {
        NameStartsWith,
        NameEndsWith,
        NameContains,
        NamespaceStartsWith,
        NamespaceEndsWith,
        NamespaceContains
    }
}