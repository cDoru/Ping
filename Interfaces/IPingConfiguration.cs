namespace PingExperiment.Interfaces
{
    public interface IPingConfiguration
    {
        string Url { get; }
        int Timeout { get; }
        int Pings { get; }
        double MaxNetworkInterfaceUsage { get; }
        double SecondsBetweenPings { get; }
    }
}