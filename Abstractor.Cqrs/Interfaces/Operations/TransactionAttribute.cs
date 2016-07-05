using System;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Especifica que um comando deve ser executado em uma transação.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TransactionAttribute : Attribute
    {
    }
}