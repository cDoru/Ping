using System;
using PingExperiment.Interfaces;
using PingExperiment.IOC;

namespace PingExperiment
{
    class Program : IResolver
    {
        static void Main(string[] args)
        {
            new Program().Execute(args);
        }

        void Execute(string[] args)
        {
            var ping = this.GetImplementation<IPing>();
            Console.WriteLine("Ping initialized");
            var result = ping.TestPing();
            Console.ReadKey();
        }
    }
}
