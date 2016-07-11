using System;
using System.Linq.Expressions;

namespace Abstractor.Cqrs.Infrastructure.CompositionRoot
{
    internal static class PredicateBuilder
    {
        internal static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        internal static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}