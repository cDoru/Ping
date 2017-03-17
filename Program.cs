using System;
using PingExperiment.Interfaces;
using PingExperiment.IOC;

namespace PingExperiment
{
    class Program : IResolver
    {
        static void Main(string[] args)
        {
            var ping = new Program().GetImplementation<IPing>();

            Console.WriteLine("Ping initialized");
            var result = ping.TestPing();


            Console.ReadKey();
        }
    }
}
