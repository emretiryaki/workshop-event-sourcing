﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                    case Domain.Events.V1.ReviewApproved view:
                        var document = new ActiveReviewDocument
                        {
                            Id = DocumentId(view.Id),
                            Caption = view.Caption,
                            Content = view.Content,
                            Owner =  view.Owner.ToString(),
                            ReviewAt = view.ReviewAt,
                            ReviewBy = view.ReviewBy.ToString()
                        };
                        await session.StoreAsync(document);
                        break;
                    
                    //need to review again,if published befores
                    case Domain.Events.V1.CaptionAndContentChanged view:
                        
                        session.Delete(DocumentId(view.Id));
                        break;
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
