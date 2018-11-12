﻿using System;

namespace Reviews.Domain.Events.V1
{
    public class ReviewCreated
    {
        public Guid Id { set; get; }
        public string Caption { get; set; }
        public string Content { get; set; }
        public UserId Owner { get;  set; }

        public override string ToString() => $"Review {Id} created by {Owner}";      
    }
    
    public class CaptionAndContentChanged
    {
        public Guid Id { get; set; }
        public string Caption { get; set; }
        public string Content { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid Owner { get; set; }
    }

    public class ReviewPublished
    {
        public Guid Id { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}