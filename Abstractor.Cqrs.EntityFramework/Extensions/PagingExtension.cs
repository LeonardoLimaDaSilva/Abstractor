using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abstractor.Cqrs.Infrastructure.Domain.Pagination;
using DelegateDecompiler;
using DelegateDecompiler.EntityFramework;
using LinqKit;

namespace Abstractor.Cqrs.EntityFramework.Extensions
{
    /// <summary>
    ///     Paging extensions for generic IQueryable.
    /// </summary>
    public static class PagingExtension
    {
        /// <summary>
        ///     Expands, decompiles and returns the number of results of a filtered query.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="query">Query to be extended.</param>
        /// <param name="expression">Expression filter.</param>
        /// <returns></returns>
        public static int FilteredCount<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query
                .AsExpandable()
                .Decompile()
                .Count(expression);
        }

        /// <summary>
        ///     Expands, decompiles and returns the number of results of a filtered query asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="query">Query to be extended.</param>
        /// <param name="expression">Expression filter.</param>
        /// <returns></returns>
        public static Task<int> FilteredCountAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression)
        {
            return query
                .AsExpandable()
                .DecompileAsync()
                .CountAsync(expression);
        }

        /// <summary>
        ///     Expands, decompiles and apply the pagination and ordering of a query.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="query">Query to be extended.</param>
        /// <param name="filter">Contains the property and direction of sorting.</param>
        /// <param name="expression">Expression filter.</param>
        /// <returns></returns>
        public static IEnumerable<T> Page<T>(
            this IQueryable<T> query,
            PagedFilter filter,
            Expression<Func<T, bool>> expression)
        {
            var result = query.AsExpandable()
                              .Where(expression)
                              .OrderByField(filter.OrderProperty, filter.OrderDirection)
                              .Skip(filter.Skip);

            if (filter.Take.HasValue)
                result = result.Take(filter.Take.Value);

            return result.Decompile().ToList();
        }

        /// <summary>
        ///     Expands, decompiles and apply the pagination and ordering of a query asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="query">Query to be extended.</param>
        /// <param name="filter">Contains the property and direction of sorting.</param>
        /// <param name="expression">Expression filter.</param>
        /// <returns></returns>
        public static Task<List<T>> PageAsync<T>(
            this IQueryable<T> query,
            PagedFilter filter,
            Expression<Func<T, bool>> expression)
        {
            var result = query.AsExpandable()
                              .Where(expression)
                              .OrderByField(filter.OrderProperty, filter.OrderDirection)
                              .Skip(filter.Skip);

            if (filter.Take.HasValue)
                result = result.Take(filter.Take.Value);

            return result.DecompileAsync().ToListAsync();
        }
    }
}