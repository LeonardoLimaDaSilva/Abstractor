using System;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    ///     Exceção que deve ser lançada quando uma entidade não for encontrada.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }
}