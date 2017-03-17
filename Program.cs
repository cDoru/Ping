using System;
using System.Net;
using PingExperiment.IOC;

namespace PingExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            const string url = "http://www.google.com";
            const int timeout = 2;
            const int pings = 20;
            const double maxNetworkUsage = 0.7;
            const double secondsBetweenPings = 1;

            AutofacConfiguration.Configure(url, timeout, pings, maxNetworkUsage, secondsBetweenPings);

            Console.ReadKey();
        }
    }
}
