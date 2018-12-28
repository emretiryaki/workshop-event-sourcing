using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Reviews.Core.Snapshots.Providers
{
    public class RedisSnapshotStoreProvider : ISnapshotStore
    {
        private readonly IRedisClientsManager clientsManager = null;

        protected RedisSnapshotStoreProvider()
        {
            clientsManager = RedisConnection.GetClientManager();
        }
        public Task<Snapshot> GetSnapshotAsync<T>(Type type, Guid aggregateId)
        {
            Snapshot snapshot = null;

            using (var redis = clientsManager.GetClient())
            {
                var strSnapshot = redis.GetValue(aggregateId.ToString());

                if (string.IsNullOrEmpty(strSnapshot)==false)
                {
                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    snapshot = JsonConvert.DeserializeObject<Snapshot>(strSnapshot, serializerSettings);
                }
            }

            return Task.FromResult(snapshot);
        }

        public Task<long> SaveSnapshotAsync(Snapshot snapshot)
        {
            using (IRedisClient redis = clientsManager.GetClient())
            {

                JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                var strSnapshot = JsonConvert.SerializeObject(snapshot, serializerSettings);

                redis.SetValue(snapshot.AggregateId.ToString(), strSnapshot);

            }

            return Task.FromResult(long.MaxValue);
        }
        
        
        private IRedisClientsManager GetClientsManager()
        {
            return RedisConnection.GetClientManager();
        }
    }
    public static class RedisConnection
    {
        private const string RedisConnectionString = "EventSourcingTest:127.0.0.1:12322";

        private static IRedisClientsManager _manager;

        public static IRedisClientsManager GetClientManager()
        {
            if (_manager == null)
            {
                _manager = new RedisManagerPool(RedisConnectionString);
            }

            return _manager;
        }

        public static void CloseClients()
        {
            if (_manager != null)
            {
                _manager.Dispose();
                _manager = null;
            }
        }

    }
}
