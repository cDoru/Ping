using System;
using Autofac;
using PingExperiment.Attributes;
using PingExperiment.Implementations;
using PingExperiment.Interfaces;
using PingExperiment.Utils;

namespace PingExperiment.IOC
{
    public static class AutofacConfiguration
    {
        private static bool Initialized;
        private static readonly object LockObject = new object();
        private static IContainer Container { get; set; }

        private static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AppConfigConfigurationProvider>().As<IConfigurationProvider>().SingleInstance();
            builder.RegisterType<PingConfiguration>().As<IPingConfiguration>();
            builder.RegisterType<Ping>().As<IPing>().InstancePerLifetimeScope();

            Container = builder.Build();
        }

        public static T GetImplementation<[IsInterface]T>(this IResolver obj) where T: class
        {
            if (!Initialized)
            {
                using (new LockUtil(LockObject))
                {
                    if (!Initialized)
                    {
                        var checks = new NctChecks();
                        checks.Check();

                        Configure();
                        Initialized = true;
                    }
                }
            }

            // resolve 
            Type typeParameterType = typeof(T);
            return (T) Container.Resolve(typeParameterType);
        }
    }
}
