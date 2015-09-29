using Abstractor.Cqrs.Interfaces.CrossCuttingConcerns;

namespace Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns
{
    /// <summary>
    /// Validador vazio utilizado quando nenhuma outra implementação for explicitamente definida.
    /// </summary>
    public sealed class EmptyValidator : IValidator
    {
        public void Validate(object instance)
        {
        }
    }
}