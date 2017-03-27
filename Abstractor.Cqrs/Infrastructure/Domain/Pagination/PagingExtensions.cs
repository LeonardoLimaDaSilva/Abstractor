using System.Linq;
using System.Linq.Expressions;

namespace Abstractor.Cqrs.Infrastructure.Domain.Pagination
{
    /// <summary>
    ///     Paging extensions for a generic IQueryable.
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        ///     Creates a dynamic query ordered by determined field.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="q">Query to be extended.</param>
        /// <param name="property">Nome of the property to be sorted.</param>
        /// <param name="direction">The direction of sorting.</param>
        /// <returns></returns>
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string property, OrderDirection direction)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, property);
            var exp = Expression.Lambda(prop, param);
            var method = direction == OrderDirection.Ascending ? "OrderBy" : "OrderByDescending";
            var types = new[] {q.ElementType, exp.Body.Type};
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}