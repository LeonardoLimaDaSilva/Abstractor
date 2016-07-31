using System.Collections.Generic;
using Abstractor.Cqrs.Infrastructure.CrossCuttingConcerns;
using Abstractor.Cqrs.Infrastructure.Domain;
using Abstractor.Cqrs.Interfaces.Events;
using SharpTestsEx;
using Xunit;

namespace Abstractor.Cqrs.Test.CrossCuttingConcerns
{
    public class JsonLoggerSerializerTests
    {
        public class EventWithSelfReferencingLoop : IDomainEvent
        {
            public FakeAggregate FakeAggregate { get; }

            public EventWithSelfReferencingLoop(FakeAggregate fakeAggregate)
            {
                FakeAggregate = fakeAggregate;
            }
        }

        public class FakeAggregateId : ValueObject<FakeAggregateId>
        {
            public int Value { get; }

            public FakeAggregateId(int value)
            {
                Value = value;
            }

            protected override IEnumerable<object> GetAttributesToIncludeInEqualityCheck()
            {
                yield return Value;
            }
        }

        public class FakeAggregate : AggregateRoot<FakeAggregateId>
        {
            public string Property { get; }

            public FakeAggregate(FakeAggregateId fakeAggregateId, string property)
                : base(fakeAggregateId)
            {
                Property = property;

                Emit(new EventWithSelfReferencingLoop(this));
            }
        }

        [Fact]
        public void Serialize_EventWithSelfReferencingLoop_ShouldIgnoreTheLoopAndSerialize()
        {
            // Arrange

            var fakeClass = new FakeAggregate(new FakeAggregateId(0), "Property");

            var selfReferencing = new EventWithSelfReferencingLoop(fakeClass);

            var serializer = new JsonLoggerSerializer();

            // Act

            var json = serializer.Serialize(selfReferencing);

            json.Should().Be("{\r\n  \"FakeAggregate\": {\r\n    \"Property\": \"Property\",\r\n    " +
                             "\"EmittedEvents\": [\r\n      {}\r\n    ],\r\n    \"Id\": {\r\n      " +
                             "\"Value\": 0\r\n    }\r\n  }\r\n}");
        }
    }
}