using System;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;

namespace PingExperiment.Entities
{
    [Serializable]
    public class PingResultEntry : ISerializable
    {
        public double? Rtt { get; private set; }
        private IPStatus? IpStatus { get; set; }
        public PingResultEntryStatus Status { get; private set; }
        public DateTime Time { get; private set; }

        public PingResultEntry(double? rtt, IPStatus? ipStatus, PingResultEntryStatus status, DateTime time)
        {
            IpStatus = ipStatus;
            Rtt = rtt;
            Status = status;
            Time = time;
        }

        // Implement this method to serialize data. The method is called on serialization.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Rtt", Rtt, typeof(double?));
            info.AddValue("IpStatus", IpStatus, typeof(IPStatus?));
            info.AddValue("Status", Status, typeof(PingResultEntryStatus));
            info.AddValue("Time", Time.Ticks, typeof(long));
        }

        // The special constructor is used to deserialize values.
        protected PingResultEntry(SerializationInfo info, StreamingContext context)
        {
            Rtt = (double?)info.GetValue("Rtt", typeof(double?));
            IpStatus = (IPStatus?)info.GetValue("IpStatus", typeof(IPStatus?));
            Status = (PingResultEntryStatus)info.GetValue("Status", typeof(PingResultEntryStatus));
            Time = new DateTime((long)info.GetValue("Time", typeof(long)));
        }
    }
}