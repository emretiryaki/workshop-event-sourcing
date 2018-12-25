using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EventStore.ClientAPI;
using Reviews.Service.WebApi;

namespace Reviews.Core.EventStore.Tests
{
    public class GesBaseTest
    {
        protected IEventStoreConnection Connection { get; }
        protected ISerializer Serializer { get; }
        protected EventTypeMapper EventTypeMapper { get; set; }
        protected Fixture AutoFixture { get; }

        protected GesBaseTest()
        {
            Connection = GetConnection().GetAwaiter().GetResult();
            Serializer = new JsonNetSerializer();
            AutoFixture = new Fixture();
            EventTypeMapper  = new EventTypeMapper()
                .Map<Domain.Events.V1.ReviewCreated>("reviewCreated")
                .Map<Domain.Events.V1.CaptionAndContentChanged>("reviewUpdated")
                .Map<Domain.Events.V1.ReviewPublished>("reviewPublished")
                .Map<Domain.Events.V1.ReviewApproved>("reviewApproved")
                .Map<Domain.ReviewSnapshot>("reviewSnapshot");

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
    }
}