namespace PingExperiment.Entities
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