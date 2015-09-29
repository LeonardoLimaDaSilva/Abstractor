namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    /// Manipula a <typeparamref name="TQuery"/> com o retorno <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">A consulta.</typeparam>
    /// <typeparam name="TResult">O tipo de retorno da <typeparamref name="TQuery"/>.</typeparam>
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Manipula a <typeparamref name="TQuery"/>. 
        /// </summary>
        /// <param name="query">A consulta.</param>
        /// <returns>Retorna o <typeparamref name="TResult"/>.</returns>
        TResult Handle(TQuery query);
    }
}