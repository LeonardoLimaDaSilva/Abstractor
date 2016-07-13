using System;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    /// <summary>
    ///     Marks that the handler execution should be logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LogAttribute : Attribute
    {
    }
}