using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions.Documents;

namespace Reviews.Core.Projections.RavenDb
{
    internal class CheckpointDocument
    {
        public object Checkpoint { get; set; }
    }

    public static class DocumentSessionExtentions
    {
        public static async Task Update<T>(this IAsyncDocumentSession session,string id,Action<T> handle,bool isThrowNeeded=false)
        {
            var doc = await session.LoadAsync<T>(id);
            
            if (isThrowNeeded && doc == null)
                throw new DocumentDoesNotExistException(typeof(T).Name, id);
            
            if (doc != null) handle(doc);
        }
    }
    
}