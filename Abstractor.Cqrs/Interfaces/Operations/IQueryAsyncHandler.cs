using System.Threading.Tasks;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    /// Manipula a <typeparamref name="TQuery"/> com o retorno <typeparamref name="TResult"/> de forma assíncrona.
    /// </summary>
    /// <typeparam name="TQuery">A consulta.</typeparam>
    /// <typeparam name="TResult">O tipo de retorno da <typeparamref name="TQuery"/>.</typeparam>
    public interface IQueryAsyncHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Manipula a <typeparamref name="TQuery"/> de forma assíncrona.
        /// </summary>
        /// <param name="query">A consulta.</param>
        /// <returns>Retorna o uma tarefa do tipo <typeparamref name="TResult"/>.</returns>
        Task<TResult> HandleAsync(TQuery query);
    }
}