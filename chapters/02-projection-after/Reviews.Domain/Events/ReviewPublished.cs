using System;

namespace Reviews.Domain.Events.V1
{
    public class ReviewPublished	
    {	
        public Guid Id { get; set; }	
        public DateTime PublishAt { get; set; }
        public Guid OwnerId { get; set; }
    }
}