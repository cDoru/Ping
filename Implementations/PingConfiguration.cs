using PingExperiment.Interfaces;

namespace PingExperiment.Implementations
{
    public class PingConfiguration : IPingConfiguration
    {
        public PingConfiguration(IConfigurationProvider provider)
        {
            provider.Ingest<string>(c => Url = c, ConfigurationSettingsHolder.Url);
            provider.Ingest<int>(c => Timeout = c, ConfigurationSettingsHolder.Timeout);
            provider.Ingest<int>(c => Pings = c, ConfigurationSettingsHolder.Pings);
            provider.Ingest<double>(c => MaxNetworkInterfaceUsage = c, ConfigurationSettingsHolder.MaxNetworkUsage);
            provider.Ingest<double>(c => SecondsBetweenPings = c, ConfigurationSettingsHolder.SecondsBetweenPings);
        }

        public string Url { get; private set; }
        public int Timeout { get; private set; }
        public int Pings { get; private set; }
        public double MaxNetworkInterfaceUsage { get; private set; }
        public double SecondsBetweenPings { get; private set; }
    }
}
