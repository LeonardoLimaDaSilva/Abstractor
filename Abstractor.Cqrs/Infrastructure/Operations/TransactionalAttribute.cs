using System;

namespace Abstractor.Cqrs.Infrastructure.Operations
{
    /// <summary>
    ///     Marks that a command should be executed transactionally.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TransactionalAttribute : Attribute
    {
    }
}