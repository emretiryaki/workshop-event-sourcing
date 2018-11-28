using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Reviews.Core.Snapshots.Providers.Util;

namespace Reviews.Core.Snapshots.Providers.InMemory
{
    public class InMemorySnapshotStorageProvider : ISnapshotStore
    {

        private readonly Dictionary<Guid, Snapshot> _items = new Dictionary<Guid, Snapshot>();

        private readonly string _memoryDumpFile;

        public InMemorySnapshotStorageProvider(string memoryDumpFile)
        {
            _memoryDumpFile = memoryDumpFile;

            if (File.Exists(_memoryDumpFile))
            {
                _items = SerializerHelper.LoadListFromFile<Dictionary<Guid, Snapshot>>(_memoryDumpFile).First();
            }
        }

        public async Task<Snapshot> GetSnapshotAsync<T>(Type type,Guid aggregateId)
        {
            if (_items.ContainsKey(aggregateId))
            {
                return _items[aggregateId];
            }
            else
            {
                return null;
            }
        }

        public async Task<long> SaveSnapshotAsync(Snapshot snapshot)
        {
            if (_items.ContainsKey(snapshot.AggregateId))
            {
                _items[snapshot.AggregateId] = snapshot;
            }
            else
            {
                _items.Add(snapshot.AggregateId, snapshot);
            }

            SerializerHelper.SaveListToFile(_memoryDumpFile, new[] {_items});
            return 1;
        }

    }
}