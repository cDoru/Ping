using PingExperiment.Interfaces;

namespace PingExperiment.Implementations
{
    public class PingConfiguration : IPingConfiguration
    {
        public PingConfiguration(IConfigurationProvider provider)
        {
            provider.Ingest<string>(c => Url = c, Settings.Url);
            provider.Ingest<int>(c => Timeout = c, Settings.Timeout);
            provider.Ingest<int>(c => Pings = c, Settings.Pings);
            provider.Ingest<double>(c => MaxNetworkInterfaceUsage = c, Settings.MaxNetworkUsage);
            provider.Ingest<double>(c => SecondsBetweenPings = c, Settings.SecondsBetweenPings);
        }

        public string Url { get; private set; }
        public int Timeout { get; private set; }
        public int Pings { get; private set; }
        public double MaxNetworkInterfaceUsage { get; private set; }
        public double SecondsBetweenPings { get; private set; }
    }
}
