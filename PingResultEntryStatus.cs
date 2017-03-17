namespace PingExperiment
{
    public enum PingResultEntryStatus
    {
        Success,
        GenericFailureSeeReplyStatus,
        PingAbortedForHighNetworkUsage,
        PingAbortedUnableToGetNetworkUsage,
        ExceptionRaisedDuringPing
    };
}