using System.Threading.Tasks;

namespace Reviews.Core.Projections
{
    public interface ICheckpointStore
    {
        Task<T> GetLastCheckpoint<T>(string projection);

        Task SetCheckpoint<T>(T checkpoint, string projection);
    }
}