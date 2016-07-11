namespace Abstractor.Cqrs.Interfaces.Operations
{
    // ReSharper disable UnusedTypeParameter
    /// <summary>
    ///     Marks a class as a query with the return of type <see cref="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">Tipo de retorno da consulta.</typeparam>
    public interface IQuery<out TResult>
    {
    }

    // ReSharper restore UnusedTypeParameter
}