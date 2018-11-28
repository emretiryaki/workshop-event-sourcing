using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace Reviews.Core.EventStore
{
    public class GesSnapshotStore : ISnapshotStore
    {
        private readonly IEventStoreConnection eventStoreConnection;
        private readonly ISerializer serializer;
        private readonly EventTypeMapper eventTypeMapper;
        private readonly GetStreamName getStreamName;
        
        private readonly UserCredentials userCredentials;
        
        
        public GesSnapshotStore(IEventStoreConnection eventStoreConnection, 
            ISerializer serializer,
            EventTypeMapper eventTypeMapper,
            GetStreamName getStreamName,
            UserCredentials userCredentials=null)
        {
            this.eventStoreConnection = eventStoreConnection ?? throw new ArgumentNullException(nameof(eventStoreConnection));;
            this.serializer = serializer ?? throw new ArgumentException(nameof(serializer));
            this.eventTypeMapper = eventTypeMapper ?? throw new ArgumentException(nameof(eventTypeMapper));
            this.getStreamName = getStreamName ?? throw new ArgumentException(nameof(getStreamName));

            this.userCredentials = userCredentials;
        }

        public async Task<long> SaveSnapshotAsync(Snapshot snapshot)
        {
            var stream = getStreamName(snapshot.GetType(), snapshot.AggregateId.ToString());

            var snapshotyEvent =  new EventData(
                snapshot.Id, 
                eventTypeMapper.GetEventName(snapshot.GetType()),
                serializer.IsJsonSerializer,
                serializer.Serialize(snapshot),
                null);

            var result = await eventStoreConnection.AppendToStreamAsync(stream,ExpectedVersion.Any, snapshotyEvent);

            return result.LogPosition.CommitPosition;

        }

        public async Task<Snapshot> GetSnapshotAsync<T>(Type type,Guid aggregateId)
        {
            Snapshot snapshot = default(Snapshot);
            var stream = getStreamName(type, aggregateId.ToString());
            Console.WriteLine("getting snapshot stream name:"+stream);
            var streamEvents = await eventStoreConnection.ReadStreamEventsBackwardAsync(stream, StreamPosition.End, 1, false);
            
            Console.WriteLine("found events:"+streamEvents.Events.Length);
            if (streamEvents.Events.Any())
            {
                var result = streamEvents.Events.FirstOrDefault();

                var t = eventTypeMapper.GetEventType(result.Event.EventType);
                Console.WriteLine("Typeof"+t.FullName);
                Console.WriteLine("Typeof"+ Encoding.ASCII.GetString(result.OriginalEvent.Data));
                snapshot = (Snapshot) serializer.Deserialize(result.OriginalEvent.Data,t);
                Console.WriteLine("build snapshot:" + snapshot.AggregateId);
                return snapshot;
            }

            return null;
        }
    }
}