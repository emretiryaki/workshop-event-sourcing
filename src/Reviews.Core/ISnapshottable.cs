using System;

namespace Reviews.Core
{
    public interface ISnapshottable<T>
    {
        Snapshot TakeSnapshot();
        void ApplySnapshot(Snapshot snapshot);

        Func<bool> SnapshotFrequency();
    }
}