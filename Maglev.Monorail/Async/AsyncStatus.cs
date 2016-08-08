namespace Maglev.Monorail.Async
{
    public enum AsyncStatus
    {
        None = 0,
        Queued = 1,
        Running = 2,
        Paused = 3,
        Finished = 4,

        Canceled = 5,           // Experimental
        Failed = 6,             // Experimental

        ThrewException = 7,
        Ignore = 8
    }
}
