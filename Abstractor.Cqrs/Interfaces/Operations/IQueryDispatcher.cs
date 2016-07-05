using System;
using System.Threading.Tasks;

namespace Abstractor.Cqrs.Interfaces.Operations
{
    /// <summary>
    ///     Processa consultas.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        ///     Executa a consulta.
        /// </summary>
        /// <typeparam name="TResult">Retorno da consulta.</typeparam>
        /// <param name="query">Consulta.</param>
        /// <returns>O objeto de retorno da consulta.</returns>
        /// <exception cref="ArgumentNullException">Se a consulta for nula.</exception>
        TResult Dispatch<TResult>(IQuery<TResult> query);

        /// <summary>
        ///     Executa a consulta de forma assíncrona.
        /// </summary>
        /// <typeparam name="TResult">Retorno da consulta.</typeparam>
        /// <param name="query">Consulta.</param>
        /// <returns>Uma tarefa do objeto de retorno da consulta.</returns>
        /// <exception cref="ArgumentNullException">Se a consulta for nula.</exception>
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);
    }
}