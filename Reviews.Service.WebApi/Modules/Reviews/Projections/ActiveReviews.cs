using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Reviews.Core.Projections;

namespace Reviews.Service.WebApi.Modules.Reviews.Projections
{
    public class ActiveReviews : Projection
    {
        private readonly Func<IAsyncDocumentSession> getSession;

        public override async Task Handle(object e)
        {
            throw new System.NotImplementedException();
        }
        
        private static string DocumentId(Guid id) => $"ActiveReviews/{id}";
    }
}
