using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Reviews.Core.Projections;

namespace Reviews.Service.WebApi.Modules.Reviews.Projections
{
    public class ActiveReviews : Projection
    {
        private readonly Func<IAsyncDocumentSession> getSession;

        public ActiveReviews(Func<IAsyncDocumentSession> session)=> getSession = session;

        public override async Task Handle(object e)
        {
            using (var session = getSession())
            {
                switch (e)
                {
                    //create active review document when the review is approved by reviewer.
                    //the activeReviewDocument will contain only approved reviews...

                    //TODO:
                    
                    
                    //second case when the document changed by review owner, the activedocument will be deleted.
                    
                    //TODO:
                }
                
                await session.SaveChangesAsync() ;
            }
        }
        
        private static string DocumentId(Guid id) => $"ActiveReviews/{id}";
    }
    
    public class ActiveReviewDocument
    {
        public string Id { get; set; }
        
        public string Caption { get; set; }

        public string Content { get; set; }

        public string Owner { get; set; }
        
        public string ReviewBy { get;  set; }
        
        public DateTime ReviewAt { get; set; }

    }
}
