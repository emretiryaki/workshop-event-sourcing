﻿using System;
using System.Collections.Generic;
using Reviews.Core;


namespace Reviews.Domain
{
    
    /// <summary>
    /// (Create || Update ) => Draft
    /// (Publish) => PendingApprove
    /// (Approve) => Approved
    /// (Reject) => Rejected
    /// (Update if current status is approved or rejected) => PendingApprove
    /// </summary>
    public enum Status
    {
        Draft,
        PendingApprove,
        Approved,
        Rejected
    }

    public class History
    {
        public History(DateTime reviewAt,Guid reviewer,Status status)
        {
            ReviewAt = reviewAt;
            Reviewer = reviewer;
            Action = status;
        }
        public Guid Reviewer { get; }
        public DateTime ReviewAt { get; }
        public Status Action { get; }
    }
    
    public class Review :Aggregate
    {
        public string Caption { get; private set; }
        public string Content { get; private set; }
        public Status CurrentStatus { get; private set; }
        public UserId Owner { get; private set; }
        private IList<History> History {get; set; } = new List<History>();

        public IList<History> GetHistory() => History;
        
        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ReviewCreated x:
                    Id = x.Id;
                    Caption = x.Caption;
                    Content = x.Content;
                    CurrentStatus = Status.Draft;
                    Owner = x.Owner;
                    break;
                case Events.V1.ReviewApproved x:
                    CurrentStatus = Status.Approved;
                    History.Add(new History(x.ReviewAt,x.ReviewBy,Status.Approved));
                    break;
                
                case Events.V1.CaptionAndContentChanged x:
                    Caption = x.Caption;
                    Content = x.Content;
                    CurrentStatus = Status.Draft;
                    
                    if(CurrentStatus == Status.Approved || CurrentStatus==Status.Rejected)
                        CurrentStatus = Status.PendingApprove;
                    
                    break;
                
                case Events.V1.ReviewPublished x:
                    CurrentStatus = Status.PendingApprove;
                    break;
            } 
        }

        public static Review Create(Guid id,UserId ownerid, string caption, string context)
        {
            var review = new Review();
            
            review.Apple(new Events.V1.ReviewCreated
            {
                Id = id,
                Caption = caption,
                Content = context,
                Owner = ownerid
            });
            return review;
        }

        public void UpdateCaptionAndContent(string caption, string content,DateTime changedAt)
        {
            if (Version == -1)
                throw new ReviewNotFoundException(Id);
            
            Apple(new Events.V1.CaptionAndContentChanged
            {
                Id=Id,
                Caption=caption,
                Content=content,
                ChangedAt=changedAt,
                Owner = Owner
            });
        }
        public void Publish()
        {
            if (Version == -1)
                throw new ReviewNotFoundException(Id);
            if (CurrentStatus == Status.Draft || CurrentStatus == Status.Rejected)
            {
                Apple(new Events.V1.ReviewPublished
                {
                    Id=Id,
                    ChangedAt=DateTime.UtcNow
                });    
            }
            
        }        
        public void Approve(UserId reviewBy, DateTime reviewAt)
        {
            if (Version == -1)
                throw new ReviewNotFoundException(Id);

            if (CurrentStatus != Status.PendingApprove)
            {
                throw new Exception($"you can't approve thats. Review : {Id}-V:{Version}  Status:{CurrentStatus}");
            }
            Apple(new Events.V1.ReviewApproved
            {
                Id = Id,
                ReviewBy = reviewBy,
                ReviewAt = reviewAt
            });
        }
    }
}

