using System;
using Reviews.Core;

namespace Reviews.Domain
{
    public class ReviewSnapshot : Snapshot
    {
        public ReviewSnapshot(Guid id, Guid aggregateId, long version,string caption,string content,Status status)
            :base(id,aggregateId,version)
        {
            Caption = caption;
            Content = content;
            CurrentStatus = status;
        }
        public string Caption { get; }
        public string Content { get; }
        public Status CurrentStatus { get; }
    }
}