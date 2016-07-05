using System.Collections.Generic;
using System.Linq;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    ///     Um objeto de valor que pode ser comparado atrav�s das suas propriedades.
    /// </summary>
    /// <typeparam name="T">Tipo do objeto de valor.</typeparam>
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        /// <summary>
        ///     Atributos que ser�o comparados para determinar igualdade.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetAttributesToIncludeInEqualityCheck();

        /// <summary>
        ///     Determina se o objeto possui o mesmo valor que o outro.
        /// </summary>
        /// <param name="other">Objeto que ser� comparado � inst�ncia atual.</param>
        /// <returns>Verdadeiro se os objetos possuem o mesmo valor.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as T);
        }

        /// <summary>
        ///     Determina se o objeto possui o mesmo valor que o outro.
        /// </summary>
        /// <param name="other">Objeto que ser� comparado � inst�ncia atual.</param>
        /// <returns>Verdadeiro se os objetos possuem o mesmo valor.</returns>
        public bool Equals(T other)
        {
            return other != null && GetAttributesToIncludeInEqualityCheck()
                .SequenceEqual(other.GetAttributesToIncludeInEqualityCheck());
        }

        /// <summary>
        ///     Determina se os objetos possuem o mesmo valor.
        /// </summary>
        /// <param name="left">Objeto da esquerda.</param>
        /// <param name="right">Objeto da direita.</param>
        /// <returns></returns>
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Determina se os objetos n�o possuem o mesmo valor.
        /// </summary>
        /// <param name="left">Objeto da esquerda.</param>
        /// <param name="right">Objeto da direita.</param>
        /// <returns></returns>
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Retorna o hash code desta inst�ncia.
        /// </summary>
        /// <returns>Hash code da inst�ncia.</returns>
        public override int GetHashCode()
        {
            return GetAttributesToIncludeInEqualityCheck()
                .Aggregate(17, (current, obj) => current * 31 + obj.GetHashCode());
        }
    }
}