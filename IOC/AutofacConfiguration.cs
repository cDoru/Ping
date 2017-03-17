using Autofac;
using PingExperiment.Implementations;
using PingExperiment.Interfaces;
using Module = Autofac.Module;

namespace PingExperiment.IOC
{
    public static class AutofacConfiguration
    {
        private static bool Initialized = false;
        private static object LockObject = new object();

        public static IContainer Container { get; private set; }

        private static void Configure(string url, int timeout, int pings, double maxNetworkUsage,
            double secondsBetweenPings)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AppConfigConfigurationProvider>().As<IConfigurationProvider>().SingleInstance();
            builder.RegisterType<PingConfiguration>().As<IPingConfiguration>();
            builder.RegisterType<Ping>().As<IPing>().InstancePerLifetimeScope();

            Container = builder.Build();
        }

        public static 
    }
}
