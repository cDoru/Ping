using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingExperiment.Interfaces;

namespace PingExperiment.Implementations
{
    public class PingConfiguration : IPingConfiguration
    {
        public PingConfiguration(string url, int timeout, int pings, double maxNetworkUsage, double secondsBetweenPings)
        {
            Url = url;
            Timeout = timeout;
            Pings = pings;
            MaxNetworkInterfaceUsage = maxNetworkUsage;
            SecondsBetweenPings = secondsBetweenPings;
        }

        public string Url { get; private set; }
        public int Timeout { get; private set; }
        public int Pings { get; private set; }
        public double MaxNetworkInterfaceUsage { get; private set; }
        public double SecondsBetweenPings { get; private set; }
    }
}
