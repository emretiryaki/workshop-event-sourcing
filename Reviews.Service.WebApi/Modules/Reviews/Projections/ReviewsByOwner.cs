using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using Reviews.Core.Projections;
using Reviews.Core.Projections.RavenDb;

namespace Reviews.Service.WebApi.Modules.Reviews.Projections
{
    public class ReviewsByOwner : Projection
    {
        private readonly Func<IAsyncDocumentSession> getSession;

        public ReviewsByOwner(Func<IAsyncDocumentSession> session)=> getSession = session;
        

        public override async Task Handle(object e)
        {
            using (var session = getSession())
            {
                switch (e)
                {
                    case Domain.Events.V1.ReviewCreated ev:

                        var documentId = DocumentId(ev.Owner);
                        var document = await session.LoadAsync<ReviewsByOwnerDocument>(documentId);

                        if (document == null)
                        {
                            document = new ReviewsByOwnerDocument
                            {
                                Id = documentId,
                                ListOfReviews = new List<ReviewsByOwnerDocument.ReviewDocument>()
                            };
                            await session.StoreAsync(document);
                        }
                        
                        document.ListOfReviews.Add(new ReviewsByOwnerDocument.ReviewDocument
                        {
                            Id = ev.Id,
                            Caption = ev.Caption,
                            Content = ev.Content,
                            Status = "Draft"
                        });
                        break;
                    
                    case Domain.Events.V1.ReviewPublished ev:

                        await session.Update<ReviewsByOwnerDocument>(DocumentId(ev.OwnerId), doc =>
                        {
                            var review = doc.ListOfReviews.First(q => q.Id == ev.Id);
                            review.Status = "Published";
                        });
                        break;                    
                    case Domain.Events.V1.ReviewApproved ev:

                        await session.Update<ReviewsByOwnerDocument>(DocumentId(ev.OwnerId), doc =>
                        {
                            var review = doc.ListOfReviews.First(q => q.Id == ev.Id);
                            review.Status = "Approved";
                        });
                        break;
                    case Domain.Events.V1.CaptionAndContentChanged ev:

                        await session.Update<ReviewsByOwnerDocument>(DocumentId(ev.Owner), doc =>
                        {
                            var review = doc.ListOfReviews.First(q => q.Id == ev.Id);
                            review.Caption = ev.Caption;
                            review.Content = ev.Content;
                            review.Status = "Draft";
                        });
                        break;
                }

                await session.SaveChangesAsync();
            }
        }

        public static string DocumentId(Guid id) => $"ReviewsByOwner/{id}";
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
}