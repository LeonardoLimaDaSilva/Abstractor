using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.Operations.Decorators;
using Abstractor.Cqrs.Interfaces.Events;
using Abstractor.Cqrs.Interfaces.Operations;
using Abstractor.Cqrs.Test.Helpers;
using Moq;
using Ploeh.AutoFixture.Xunit2;
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

            commandHandler.Setup(d => d.EmittedEvents).Returns(new List<IDomainEvent>());

            // Act

            decorator.Handle(command);

            // Verify

            eventDispatcher.Verify(d => d.Dispatch(It.IsAny<IDomainEvent>()), Times.Never);
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

            commandHandler.Setup(d => d.EmittedEvents).Returns(domainEvents);

            // Act

            decorator.Handle(command);

            // Verify

            eventDispatcher.Verify(d => d.Dispatch(domainEvent1), Times.Once);
            eventDispatcher.Verify(d => d.Dispatch(domainEvent2), Times.Once);
        }
    }
}