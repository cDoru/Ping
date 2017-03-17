using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using PingExperiment.Exceptions;
using PingExperiment.Interfaces;

namespace PingExperiment
{
    internal class Ping : IPing
    {
        // Private vars ping-related
        private readonly IPAddress _remoteAddr;
        private readonly int _timeout;
        private readonly int _pingsPerTest;
        private readonly double _maxNetworkInterfaceUsagePercentage;
        private readonly double _secondsBetweenPings;

        // Reference to the network interface that will be used to ping
        private readonly NetworkInterface _ni;

        // Performance counter needed to check network bandwith usage
        private PerformanceCounter _bandwidthCounter;
        private PerformanceCounter _dataSentCounter;
        private PerformanceCounter _dataReceivedCounter;

        // Bool checked every ping loop, it can be set true from other thread if they want the test to stop as soon as possible
        private volatile bool _isStopping;

        // Lock used to provide monitor-like accass to this class
        private readonly object _lock;


        public Ping(IPingConfiguration configuration)
            : this(
                configuration.Url, configuration.Timeout, configuration.Pings, configuration.MaxNetworkInterfaceUsage,
                configuration.SecondsBetweenPings)
        {

        }

        private Ping(string url, int timeout = 2, 
            int pingsPerTest = 50, 
            double maxNetworkInterfaceUsagePercentage = 1, 
            double secondsBetweenPings = 0)
            : this(UrlToIpAddress(url), timeout, pingsPerTest, maxNetworkInterfaceUsagePercentage, secondsBetweenPings)
        {

        }

        private Ping(IPAddress remoteAddr, 
            int timeout = 2, 
            int pingsPerTest = 50, 
            double maxNetworkInterfaceUsagePercentage = 70, 
            double secondsBetweenPings = 0)
        {
            _remoteAddr = remoteAddr;
            _timeout = timeout;
            _pingsPerTest = pingsPerTest;
            _maxNetworkInterfaceUsagePercentage = maxNetworkInterfaceUsagePercentage;
            _secondsBetweenPings = secondsBetweenPings;

            _lock = new object();
            _ni = GetNetworkInterface();
        }

        private static IPAddress UrlToIpAddress(string url)
        {
            var regexHelper = new RegexHelper();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                throw new UrlFormatDeniedException("url must start with either https:// or http://");

            if (url.StartsWith("www."))
                throw new UrlFormatDeniedException("url must start with http:// or https:// ");

            if (!regexHelper.IsValid(url))
            {
                throw new UrlFormatInvalidException("Url is malformed");
            }

            Uri uri;

            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                throw new UriFormatInvalidException("Could not construct Uri from url", ex);
            }

            IPAddress address;
            try
            {
                address = Dns.GetHostAddresses(uri.Host)[0];
            }
            catch (Exception ex)
            {
                throw new DnsResolveFailureException("Dns could not resolve the provided url", ex);
            }

            return address;
        }

        public PingResult TestPing()
        {
            lock (_lock)
            {
                _isStopping = false;

                var pingSender = new System.Net.NetworkInformation.Ping();
                var result = new PingResult();

                for (var i = 0; i < _pingsPerTest; i++)
                {
                    if (_isStopping == false)
                    {
                        // Checking the network usage...
                        var check = CheckNetworkUsage();
                        switch (check)
                        {
                            case (NetworkUsageResult.Good): // Ok, path clear, start pinging
                                try
                                {
                                    var reply = pingSender.Send(_remoteAddr, _timeout);
                                    if (reply != null)
                                    {
                                        if (reply.Status == IPStatus.Success)
                                        {
                                            // All has gone well
                                            result.AddPingResultEntry(new PingResultEntry(
                                                reply.RoundtripTime, reply.Status, PingResultEntryStatus.Success, DateTime.Now));
                                        }
                                        else
                                        {
                                            // Something went wrong, wrong but "expected"
                                            result.AddPingResultEntry(new PingResultEntry(
                                                reply.RoundtripTime, reply.Status, PingResultEntryStatus.GenericFailureSeeReplyStatus, DateTime.Now));
                                        }
                                    }
                                    else
                                        throw new NullReferenceException("reply");
                                }
                                catch
                                {
                                    // Something went really wrong, and we should "prepare for unexpected consequences"... oh, we did it with this catch
                                    result.AddPingResultEntry(new PingResultEntry(
                                        null, null, PingResultEntryStatus.ExceptionRaisedDuringPing, DateTime.Now));
                                }
                                break;
                            case NetworkUsageResult.Crowded:
                                // Network usage was too high to give meaningful results
                                result.AddPingResultEntry(new PingResultEntry(
                                    null, null, PingResultEntryStatus.PingAbortedForHighNetworkUsage, DateTime.Now));
                                break;
                            case NetworkUsageResult.UnableToTest:
                                // Something went wrong in checking the network
                                result.AddPingResultEntry(new PingResultEntry(
                                    null, null, PingResultEntryStatus.PingAbortedUnableToGetNetworkUsage, DateTime.Now));
                                break;
                        }

                        // Checking again if anyone asked us to stop and also avoid waiting at the end of the last loop
                        if (_isStopping == false && i < _pingsPerTest - 1)
                        {
                            // Ok, i've done one single ping, now i should wait the time the user setted before doing it again
                            //System.Threading.Thread.Sleep(secondsBetweenPings * 1000);
                            Monitor.Wait(_lock, (int)(_secondsBetweenPings * 1000));
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return result;
            }
        }

        // Used to stop the ping loop before it has done all scheduled loops
        // ReSharper disable once UnusedMember.Local
        public void SkipRemainingPingsOnce()
        {
            lock (_lock)
            {
                _isStopping = true;
                Monitor.Pulse(_lock);
            }
        }

        // Check if network bandwith usage % is lower (or not) then the value given by the user
        private NetworkUsageResult CheckNetworkUsage()
        {
            // If it we can't get a ref to the network interface there will be no need for performance counters 'cause we wont be able to use them
            if (_ni != null && _bandwidthCounter == null && _dataSentCounter == null && _dataReceivedCounter == null)
            {
                try
                {
                    _bandwidthCounter = new PerformanceCounter("Network Interface", "Current Bandwidth", _ni.Description);
                    _dataSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", _ni.Description);
                    _dataReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", _ni.Description);
                }
                catch
                {
                    // Not sure what to do here
                }
            }

            // If we have successfully created the performance counters
            if (_bandwidthCounter != null && _dataSentCounter != null && _dataReceivedCounter != null)
            {
                // Do the check...
                return (GetNetworkUtilization() < _maxNetworkInterfaceUsagePercentage)
                    ? NetworkUsageResult.Good
                    : NetworkUsageResult.Crowded;
            }
            
            // If not, we can't test
            return NetworkUsageResult.UnableToTest;
        }

        /* 
         * As suggested somewhere on stackoverflow.com, this function tries to open an UDP connection
         * to the remote host we want to ping so we can get our local address and find the interface that uses it
         */
        private static NetworkInterface GetNetworkInterface()
        {
            return (from nic in NetworkInterface.GetAllNetworkInterfaces()
                let ipProps = nic.GetIPProperties()
                from ip in ipProps.UnicastAddresses
                where (nic.OperationalStatus == OperationalStatus.Up) && (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                select nic).FirstOrDefault();
        }

        /* 
         * As suggested somewhere on stackoverflow.com, this function use the infos provided by the performance counters
         * to stimate the current percentage usage of the network interface. Keep in mind that the % is over the speed
         * of the interface, which is NOT the same as the bandwith your IPS provides you. 
         * Eg:
         * - IPS gives you 7MB adsl
         * - You are connected to your router via ethernet 100MB
         * --> 100MB is the important value, max possible interface usage for you will be around 7%
         * Selecting a max testable usage of 1% is probably good to prevent your ping from being ruined.
         */
        private double GetNetworkUtilization()
        {
            const int numberOfIterations = 10;

            var bandwidth = _bandwidthCounter.NextValue();

            float sendSum = 0;
            float receiveSum = 0;

            for (var index = 0; index < numberOfIterations; index++)
            {
                sendSum += _dataSentCounter.NextValue();
                receiveSum += _dataReceivedCounter.NextValue();
            }

            var dataSent = sendSum;
            var dataReceived = receiveSum;

            double utilization = (8 * (dataSent + dataReceived)) / (bandwidth * numberOfIterations) * 100;
            return utilization;
        }
    }
}