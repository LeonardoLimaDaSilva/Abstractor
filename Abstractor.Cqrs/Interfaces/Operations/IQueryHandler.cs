namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Handler for the query that implements <see cref="IQuery{TResult}" />
    /// </summary>
    /// <typeparam name="TQuery">Query to be handled.</typeparam>
    /// <typeparam name="TResult">Return type.</typeparam>
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        ///     Handles the <see cref="IQuery{TResult}" />.
        /// </summary>
        /// <param name="query">Query to be handled.</param>
        /// <returns>Returns an object of type <typeparamref name="TResult" />.</returns>
        TResult Handle(TQuery query);
    }
}