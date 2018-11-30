using System;

namespace Reviews.Domain.Events.V1
{
    public class ReviewApproved
    {
        public Guid Id { set; get; }
        public UserId ReviewBy { get;  set; }
        public DateTime ReviewAt { get; set; }
        public string Caption { get; set; }
        public string Content { get; set; }
        public Guid Owner { get; set; }

        public override string ToString() => $"Review {Id} review by {ReviewBy}";      
    }
}