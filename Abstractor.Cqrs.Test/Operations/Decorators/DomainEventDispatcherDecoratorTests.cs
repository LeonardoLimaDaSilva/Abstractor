using System.Collections.Generic;
using System.Linq;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.Operations.Decorators
{
    public class DomainEventDispatcherDecoratorTests
    {
        public interface ICommandThatPublishesDomainEvents : ICommand, IDomainEvent
        {
        }

        [Theory]
        [AutoMoqData]
        public void Handle_EmptyDomainEvents_ShouldNotDispatch(
            [Frozen] Mock<ICommandHandler<ICommandThatPublishesDomainEvents>> commandHandler,
            [Frozen] Mock<IDomainEventDispatcher> eventDispatcher,
            ICommandThatPublishesDomainEvents command,
            DomainEventDispatcherDecorator<ICommandThatPublishesDomainEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(command)).Returns(new List<IDomainEvent>());

            // Act

            var result = decorator.Handle(command);

            // Verify

            result.Should().Be.Empty();

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<IDomainEvent>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void Handle_NullDomainEvents_ReturnsNull(
            [Frozen] Mock<ICommandHandler<ICommandThatPublishesDomainEvents>> commandHandler,
            ICommandThatPublishesDomainEvents command,
            DomainEventDispatcherDecorator<ICommandThatPublishesDomainEvents> decorator)
        {
            // Arrange

            commandHandler.Setup(d => d.Handle(command)).Returns((IEnumerable<IDomainEvent>) null);

            // Act

            var result = decorator.Handle(command);

            // Verify

            result.Should().Be.Null();
        }

        [Theory]
        [AutoMoqData]
        public void Handle_Success_ShouldDispatchTheEventsReturnedFromTheCommandHandler(
            [Frozen] Mock<ICommandHandler<ICommandThatPublishesDomainEvents>> commandHandler,
            [Frozen] Mock<IDomainEventDispatcher> eventDispatcher,
            ICommandThatPublishesDomainEvents command,
            IDomainEvent domainEvent1,
            IDomainEvent domainEvent2,
            DomainEventDispatcherDecorator<ICommandThatPublishesDomainEvents> decorator)
        {
            // Arrange

            var domainEvents = new List<IDomainEvent>
            {
                domainEvent1,
                domainEvent2
            };

            commandHandler.Setup(d => d.Handle(command)).Returns(domainEvents);

            // Act

            var result = decorator.Handle(command).ToList();

            // Verify

            result[0].Should().Be(domainEvent1);
            result[1].Should().Be(domainEvent2);

            eventDispatcher.Verify(d => d.Dispatch(domainEvent1), Times.Once);
            eventDispatcher.Verify(d => d.Dispatch(domainEvent2), Times.Once);
        }
    }
}