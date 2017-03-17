using PingExperiment.Interfaces;

namespace PingExperiment.Implementations
{
    public class PingConfiguration : IPingConfiguration
    {
        public PingConfiguration(IConfigurationProvider provider)
        {
            provider.Ingest<string>(c => Url = c, Settings.Url);
        }

        public string Url { get; private set; }
        public int Timeout { get; private set; }
        public int Pings { get; private set; }
        public double MaxNetworkInterfaceUsage { get; private set; }
        public double SecondsBetweenPings { get; private set; }
    }
}
