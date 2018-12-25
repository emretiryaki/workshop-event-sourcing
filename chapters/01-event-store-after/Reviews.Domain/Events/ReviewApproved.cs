using System;

namespace Reviews.Domain.Events.V1
{
    public class ReviewApproved	
    {	
        public Guid Id { set; get; }	
        public UserId ReviewBy { get;  set; }	
        public DateTime ReviewAt { get; set; }	
        public override string ToString() => $"Review {Id} review by {ReviewBy}";      	
    }
}