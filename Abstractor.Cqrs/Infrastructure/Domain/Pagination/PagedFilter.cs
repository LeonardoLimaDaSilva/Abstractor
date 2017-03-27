namespace Abstractor.Cqrs.Infrastructure.Domain.Pagination
{
    /// <summary>
    ///     Defines the required fields for paged queries.
    /// </summary>
    public abstract class PagedFilter
    {
        private int? _page;

        /// <summary>
        ///     The order direction.
        /// </summary>
        public OrderDirection OrderDirection { get; set; }

        /// <summary>
        ///     Name of the property used for sorting.
        /// </summary>
        public string OrderProperty { get; set; }

        /// <summary>
        ///     Current page of the query.
        /// </summary>
        public int? Page
        {
            get { return _page <= 0 ? 1 : _page; }
            set { _page = value; }
        }

        /// <summary>
        ///     Number of results to be skipped.
        /// </summary>
        public int Skip => Take.HasValue
            ? (Page.GetValueOrDefault() - 1) * Take.Value
            : 0;

        /// <summary>
        ///     Number of results to be returned.
        /// </summary>
        public int? Take { get; set; }
    }
}