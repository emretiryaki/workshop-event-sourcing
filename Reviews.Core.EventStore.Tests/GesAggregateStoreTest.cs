using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EventStore.ClientAPI;
using Reviews.Service.WebApi;
using Xunit;
using FluentAssertions;
using FluentAssertions.Formatting;

namespace Reviews.Core.EventStore.Tests
{
    public class GesAggregateStoreTest
    {
        private IEventStoreConnection Connection { get; }
        private ISerializer Serializer { get; }
        private EventTypeMapper EventTypeMapper { get; }
        private Fixture AutoFixture { get; }
        
        public GesAggregateStoreTest()
        {
            Connection = GetConnection().GetAwaiter().GetResult();
            Serializer = new JsonNetSerializer();
            AutoFixture = new Fixture();

            EventTypeMapper = new EventTypeMapper()
                .Map<Domain.Events.V1.ReviewCreated>("reviewCreated");

        }
        
        private static async Task<IEventStoreConnection> GetConnection()
        {
            var connection = EventStoreConnection.Create(
                new IPEndPoint(IPAddress.Loopback, 1113)
            );

            await connection
                .ConnectAsync()
                .ConfigureAwait(false);

            return connection;
        }
        
        [Fact]
        public async Task can_save_aggregate()
        {
            var aggregate = new Reviews.Domain.Review();

            aggregate.Apple(AutoFixture.Create<Domain.Events.V1.ReviewCreated>());


            var sut = new GesAggrigateStore(Connection, Serializer, EventTypeMapper, (a, b) => $"{a}-{b}", null);

            var result = await sut.Save(aggregate);

            result.NextExceptedVersion.Should().Be(0);
        }
        
        private Guid AggregateId { get; } = Guid.NewGuid();
        
        [Fact]
        public async Task can_load_aggregate()
        {
            var aggregate = new Reviews.Domain.Review();

            aggregate.Apple(AutoFixture.Build<Domain.Events.V1.ReviewCreated>().With(e=>e.Id,AggregateId).Create());

           
            var sut = new GesAggrigateStore(Connection, Serializer, EventTypeMapper, (a, b) => $"{a}-{b}", null);
            
            var saveResult = await sut.Save(aggregate);
            
            var result = await sut.Load<Domain.Review>(AggregateId.ToString());

            result.Id.Should().Be(AggregateId);
        }
    }
}
