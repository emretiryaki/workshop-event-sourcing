using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Reviews.Core.Projections;

namespace Reviews.Core.EventStore
{
    public class ProjectionManager
    {
        private readonly IEventStoreConnection eventStoreConnection;
        private readonly ICheckpointStore checkpointStore;
        private readonly ISerializer serializer;
        private readonly EventTypeMapper eventTypeMapper;
        private readonly Projection[] projections;
        private readonly UserCredentials userCredentials;

        private readonly int maxLiveQueueSize ;
        private readonly int readBatchSize;
        private readonly bool verboseLogging;
        

        internal ProjectionManager(IEventStoreConnection eventStoreConnection, 
                                    ICheckpointStore checkpointStore,
                                    ISerializer serializer,
                                    EventTypeMapper eventTypeMapper,
                                    Projection[] projections,
                                    int? maxLiveQueueSize,
                                    int? readBatchSize,
                                    bool? verboseLogging,
                                    UserCredentials userCredentials=null
                                  )
        {
            this.eventStoreConnection = eventStoreConnection ?? throw new ArgumentNullException(nameof(eventStoreConnection));
            this.checkpointStore = checkpointStore ?? throw new ArgumentNullException(nameof(checkpointStore));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.eventTypeMapper = eventTypeMapper ?? throw new ArgumentException(nameof(eventTypeMapper));
            this.projections = projections;
            this.userCredentials = userCredentials;

            this.maxLiveQueueSize = maxLiveQueueSize ?? 10000;
            this.readBatchSize = readBatchSize ?? 500;
            this.verboseLogging = verboseLogging ?? false;
        }

        public Task StartAll() => Task.WhenAll(this.projections.Select(StartProjection));

        private async Task StartProjection(Projection projection)
        {

            var lastCheckpoint = await checkpointStore.GetLastCheckpoint<Position>(projection);
            
            var catchUpSubscriptionSettings = new CatchUpSubscriptionSettings(maxLiveQueueSize,readBatchSize,verboseLogging,false,projection);

            eventStoreConnection.SubscribeToAllFrom(lastCheckpoint, catchUpSubscriptionSettings,
                eventAppeared(projection),
                liveProcessingStarted(projection),subscriptionDropped(projection),userCredentials );


        }

        
        //SubscribeToAllFrom(this IEventStoreConnection target,
        //                    Position? lastCheckpoint,
        //                    CatchUpSubscriptionSettings settings,
        //                    Action<EventStoreCatchUpSubscription, ResolvedEvent> eventAppeared,
        //                    Action<EventStoreCatchUpSubscription> liveProcessingStarted = null,
        //                    Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
        //                    UserCredentials userCredentials = null)
        
        private Action<EventStoreCatchUpSubscription, ResolvedEvent> eventAppeared(Projection projection)
            => async (_, e) =>
            {
                
            };

        private Action<EventStoreCatchUpSubscription> liveProcessingStarted(Projection projection) 
            => async (eventStoreCatchUpSubscription) =>
            {
                Console.WriteLine("${projection} has been started,now processing real time!");
            };

        private Action<EventStoreCatchUpSubscription, SubscriptionDropReason, Exception> subscriptionDropped(Projection projection)
            => async (eventStoreCatchUpSubscription, subscriptionDropReason, exception) =>
            {
                
            };
    }
}
