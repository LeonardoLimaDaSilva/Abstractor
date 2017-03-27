using System.Collections.Generic;

namespace Abstractor.Cqrs.Infrastructure.Domain.Pagination
{
    /// <summary>
    ///     Wrapper for the paged query result.
    /// </summary>
    /// <typeparam name="T">Result type.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        ///     List of paged results.
        /// </summary>
        public IEnumerable<T> Result { get; }

        /// <summary>
        ///     Total number of results for determined query.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        ///     Constructs the paged result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="totalCount"></param>
        public PagedResult(IEnumerable<T> result, int totalCount)
        {
            Result = result;
            TotalCount = totalCount;
        }
    }
}