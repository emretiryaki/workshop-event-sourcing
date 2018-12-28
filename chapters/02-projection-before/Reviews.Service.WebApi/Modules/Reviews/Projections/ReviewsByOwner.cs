using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Reviews.Core.Projections;

namespace Reviews.Service.WebApi.Modules.Reviews.Projections
{
    public class ReviewsByOwner : Projection
    {
        private readonly Func<IAsyncDocumentSession> getSession;

        public ReviewsByOwner(Func<IAsyncDocumentSession> session)=> getSession = session;
            
        
        public override Task Handle(object e)
        {
            throw new NotImplementedException();
        }
    }
    
    
    public class ReviewsByOwnerDocument
    {
        public string Id { get; set; }
        public IList<ReviewDocument> ListOfReviews { get; set; }

        public class ReviewDocument
        {
            public Guid Id { get; set; }
            public string Caption { get; set; }
            public string Content { get; set; }
            public string Status { get; set; }
        }

}