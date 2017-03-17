using Autofac;
using PingExperiment.Implementations;
using PingExperiment.Interfaces;

namespace PingExperiment.IOC
{
    public class AutofacConfiguration
    {
        public static IContainer Container { get; private set; }

        public static void Configure(string url, int timeout, int pings, double maxNetworkUsage,
            double secondsBetweenPings)
        {
            var builder = new ContainerBuilder();

            //builder.Register(
            //    x => new PingConfiguration(url, timeout, pings, maxNetworkUsage, secondsBetweenPings));

            builder.RegisterType<Ping>().As<IPing>().InstancePerLifetimeScope();

            Container = builder.Build();
        }
    }
}
