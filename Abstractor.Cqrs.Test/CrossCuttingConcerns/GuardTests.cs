using System;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class GuardTests
    {
        [Theory]
        [InlineData(null, null, "Valor não pode ser nulo.")]
        [InlineData(null, "message", "message")]
        [InlineData("argument", "message", "message\r\nNome do parâmetro: argument")]
        public void Guard_ValueNull_ThrowsArgumentNullException(
            string argument,
            string message,
            string expectedMessage)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Guard.ArgumentIsNotNull(null, argument, message));
            ex.ParamName.Should().Be(argument);
            ex.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Guard_EntityIsNotNull_ShouldPass()
        {
            Guard.EntityIsNotNull("value");
        }

        [Fact]
        public void Guard_EntityIsNull_ThrowsEntityNotFoundException()
        {
            Assert.Throws<EntityNotFoundException>(() => Guard.EntityIsNotNull(null));
        }

        [Fact]
        public void Guard_TypedEntityIsNull_ThrowsEntityNotFoundException()
        {
            var ex = Assert.Throws<EntityNotFoundException>(() => Guard.EntityIsNotNull<string>(null, "primaryKey"));
            ex.Message.Should().Be("A entidade do tipo 'String' não foi encontrada para a chave primária 'primaryKey'.");
        }

        [Fact]
        public void Guard_ValueNotNull_ShouldPass()
        {
            Guard.ArgumentIsNotNull("value", "argument");
        }
    }
}