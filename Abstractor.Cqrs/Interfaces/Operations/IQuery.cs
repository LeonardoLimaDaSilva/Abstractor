namespace Abstractor.Cqrs.Interfaces.Operations
{
    // ReSharper disable UnusedTypeParameter
    /// <summary>
    /// Especifica que a classe é uma consulta com um retorno do tipo <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">Tipo de retorno da consulta.</typeparam>
    public interface IQuery<out TResult>
    {
    }
    // ReSharper restore UnusedTypeParameter
}