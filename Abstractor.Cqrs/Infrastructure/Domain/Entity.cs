using System;

namespace Abstractor.Cqrs.Infrastructure.Domain
{
    /// <summary>
    /// Uma entidade que pode ser identificada através de uma chave primária.
    /// </summary>
    /// <typeparam name="TId">O tipo da chave primária.</typeparam>
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        /// <summary>
        /// Chave primária imutável da entidade.
        /// </summary>
        public TId Id { get; }

        protected Entity(TId id)
        {
            Id = id;
        }

        /// <summary>
        /// Retorna o hash code desta instância através do Id.
        /// </summary>
        /// <returns>Hash code da instância através do Id.</returns>
        public override int GetHashCode()
        {
            return IsTransient(this) ? 0 : Id.GetHashCode();
        }

        /// <summary>
        /// Determina se esta entidade é igual a outro objeto.
        /// </summary>
        /// <param name="other">Objeto que será comparado à atual instância.</param>
        /// <returns>Verdadeiro apenas se o outro objeto for um <see cref="Entity{TId}"/>, 
        /// ambos objetos não forem nulos ou transientes e ambas as entidades possuem o mesmo Id.</returns>
        public override bool Equals(object other)
        {
            return Equals(other as Entity<TId>);
        }

        /// <summary>
        /// Determina se esta entidade é igual à outra.
        /// </summary>
        /// <param name="other">Entidade que será comparada à atual.</param>
        /// <returns>Verdadeiro se a outra entidade não for nula, nem transiente, e possuir o mesmo Id da entidade atual.</returns>
        public virtual bool Equals(Entity<TId> other)
        {
            // uma instância nunca será igual a nulo
            if (other == null) return false;

            // quando a referência é igual, eles são o mesmo objeto
            if (ReferenceEquals(this, other)) return true;

            // retorna falso quando um objeto é transiente ou os ids não são iguais
            if (IsTransient(this) || IsTransient(other) || !Equals(Id, other.Id)) return false;

            // quando os ids são iguais mas o objeto é transiente retorna true caso o objeto 
            // possa ser convertido em outro a instância pode ter sido gerada por um proxy
            var otherType = other.GetUnproxiedType();
            var thisType = GetUnproxiedType();
            return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
        }

        private static bool IsTransient(Entity<TId> obj)
        {
            // um objeto é transiente quando seu id é default
            return Equals(obj.Id, default(TId));
        }

        private Type GetUnproxiedType()
        {
            // returna o tipo subjacente, caso tenha sido gerado por um proxy
            return GetType();
        }
    }
}
