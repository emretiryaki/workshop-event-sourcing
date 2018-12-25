using System;
using System.Threading.Tasks;

namespace Reviews.Core
{
    public interface ISnapshotStore{
        
        Task<Snapshot> GetSnapshotAsync<T>(Type type,Guid aggregateId);
        Task<long> SaveSnapshotAsync(Snapshot snapshot);
    }
}