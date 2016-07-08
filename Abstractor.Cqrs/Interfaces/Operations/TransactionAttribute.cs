using System;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Marks that a command should be executed transactionally.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TransactionAttribute : Attribute
    {
    }
}