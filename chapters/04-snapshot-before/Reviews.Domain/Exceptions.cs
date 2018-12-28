using System;

namespace Reviews.Domain
{
    public class ReviewNotFoundException : Exception
    {
        public ReviewNotFoundException(Guid id) : base($"Review with id '{id}' was not found") { }
    }
}