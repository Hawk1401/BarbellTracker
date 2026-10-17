using BarbellTracker.DomainCode;

namespace BarbellTracker.ApplicationCode.Event
{
    /// <summary>A session was saved (or overwritten), e.g. so the history can refresh</summary>
    public class SessionSaved
    {
        public Session Session { get; init; }
    }

    /// <summary>The user wants to see a saved session</summary>
    public class SessionOpened
    {
        public Session Session { get; init; }
    }
}
