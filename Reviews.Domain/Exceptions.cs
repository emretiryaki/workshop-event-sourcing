using System;

namespace Reviews.Domain
{
    public class ReviewNotFoundException : Exception
    {
        public ReviewNotFoundException(Guid id) : base($"Review with id '{id}' was not found") { }
    }
    
    public class ReviewInvalidStatus : Exception
    {
        public ReviewInvalidStatus(Guid id,Status CurrentStatus) : base($"you can't approve thats. Review : {id} Status:{CurrentStatus}") { }
    }
}