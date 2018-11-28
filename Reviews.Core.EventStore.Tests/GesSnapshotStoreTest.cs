using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Reviews.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Reviews.Core.EventStore.Tests
{
    public class GesSnapshotStoreTest : GesBaseTest
    {
        private readonly ITestOutputHelper outputHelper;

        public GesSnapshotStoreTest(ITestOutputHelper outputHelper):base()
        {
            this.outputHelper = outputHelper;

            EventTypeMapper = new EventTypeMapper()
                .Map<Domain.Events.V1.ReviewCreated>("reviewCreated")
                .Map<Domain.Events.V1.ReviewApproved>("reviewApproved")
                .Map<Domain.ReviewSnapshot>("reviewSnapshot");
        }
        
        [Fact]
        public async Task can_take_snapshot()
        {
            //Given
            var aggregate = new Reviews.Domain.Review();
            aggregate.Apple(AutoFixture.Create<Domain.Events.V1.ReviewCreated>());
            aggregate.Apple(AutoFixture.Create<Domain.Events.V1.ReviewApproved>());
            
            var sut = new GesSnapshotStore(Connection, Serializer, EventTypeMapper, (a, b) => $"{a}-{b}", null);
            
            //When
            var result =await  sut.SaveSnapshotAsync(aggregate.TakeSnapshot());

            //Then
            outputHelper.WriteLine($"Snapshot result last position:{result}");
            result.Should().BeGreaterThan(0);
        }
        private Guid AggregateId { get; } = Guid.NewGuid();
        [Fact]
        public async Task can_get_snapshot()
        {
            //Given
            var aggregate = new Reviews.Domain.Review();
            aggregate.Apple(AutoFixture.Build<Domain.Events.V1.ReviewCreated>().With(e=>e.Id,AggregateId).Create());
            aggregate.Apple(AutoFixture.Build<Domain.Events.V1.ReviewPublished>().With(e=>e.Id,AggregateId).Create());
            aggregate.Apple(AutoFixture.Build<Domain.Events.V1.ReviewApproved>().With(e=>e.Id,AggregateId).Create());
            
            var sut = new GesSnapshotStore(Connection, Serializer, EventTypeMapper, (a, b) => $"{a}-{b}", null);
            await  sut.SaveSnapshotAsync(aggregate.TakeSnapshot()); 
            
            //When
            var result = await sut.GetSnapshotAsync<Review>(typeof(ReviewSnapshot), AggregateId);

            //Then
            outputHelper.WriteLine($"Snapshot result last Version:{result.Version}");
            result.AggregateId.Should().Be(AggregateId);
        }
    }
}