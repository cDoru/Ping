using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PingExperiment
{
    [Serializable]
    public class PingResult
    {
        private readonly List<PingResultEntry> _results;
        private double? _avg;
        private double? _dev;
        private double? _max;
        private double? _min;
        private DateTime? _avgTime;

        public PingResult()
        {
            _results = new List<PingResultEntry>();
        }

        // Implement this method to serialize data. The method is called on serialization.
        // ReSharper disable once UnusedParameter.Global
        // ReSharper disable once UnusedMember.Global
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("results", _results, typeof(List<PingResultEntry>));
        }

        // The special constructor is used to deserialize values.
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Global
        public PingResult(SerializationInfo info, StreamingContext context)
        {
            _results = (List<PingResultEntry>)info.GetValue("Rtt", typeof(List<PingResultEntry>));
        }

        public void AddPingResultEntry(PingResultEntry newEntry)
        {
            // Adding a new entry
            _results.Add(newEntry);
            // Reset any previously calculated stats (they where probably already null but who knows...)
            _avg = null;
            _dev = null;
            _max = null;
            _min = null;
            _avgTime = null;
        }

        public double? GetAvg()
        {
            // Calculating avg if not yet calculated
            if (_avg == null && _results.Any(res => res.Status == PingResultEntryStatus.Success))
            {
                _avg = 0;
                var cont = 0;
                foreach (var e in _results.Where(x => x.Status == PingResultEntryStatus.Success))
                {
                    cont++;
                    _avg += e.Rtt;
                }
                if (cont > 0)
                {
                    _avg = _avg / cont;
                }
            }
            return _avg;
        }

        // ReSharper disable once UnusedMember.Global
        public double? GetDev()
        {
            // Calculating dev if not yet calculated
            if (_dev != null || _results.All(res => res.Status != PingResultEntryStatus.Success)) return _dev;

            var tmpAvg = GetAvg();
            if (tmpAvg == null) return _dev;

            _dev = 0;
            var cont = 0;
            double tmpDev = 0;
            foreach (var e in _results.Where(x => x.Status == PingResultEntryStatus.Success))
            {
                cont++;
                if (e.Rtt != null) tmpDev += Math.Pow(((double)e.Rtt - (double)tmpAvg), 2);
            }
            if (cont > 0)
            {
                _dev = Math.Pow(tmpDev / cont, 0.5);
            }
            return _dev;
        }

        // ReSharper disable once UnusedMember.Global
        public double? GetMax()
        {
            // Calculating max if not yet calculated
            if (_max == null && _results.Any(res => res.Status == PingResultEntryStatus.Success))
            {
                _max = _results.Where(x => x.Status == PingResultEntryStatus.Success).OrderBy(y => y.Rtt).Last().Rtt;
            }

            return _max;
        }

        // ReSharper disable once UnusedMember.Global
        public double? GetMin()
        {
            // Calculating min if not yet calculated
            if (_min == null && _results.Any(res => res.Status == PingResultEntryStatus.Success))
            {
                _min = _results.Where(x => x.Status == PingResultEntryStatus.Success).OrderBy(y => y.Rtt).First().Rtt;
            }

            return _min;
        }

        public DateTime? GetAvgTime()
        {
            // Calculating avgTime if not yet calculated
            if (_avgTime == null && _results.Any())
            {
                _avgTime = new DateTime((_results.First().Time.Ticks + _results.Last().Time.Ticks) / 2);
            }

            return _avgTime;
        }

        // Group all the given PingResults by days (mon-sun) and convert them in AggregatedResults, which are grouped by hours (0..24)
        // ReSharper disable once UnusedMember.Global
        public static Dictionary<DayOfWeek, List<AggregatedResult>> AggregatePingResults(IEnumerable<List<PingResult>> toBeMerged)
        {
            // Flattening the list of lists
            var allResults = toBeMerged.SelectMany(x => x).Where(x => x._avg != null).ToList();

            // Prepare a new list where to put the aggregated results we are about to calculate
            var output = new Dictionary<DayOfWeek, List<AggregatedResult>>();

            // Grouping by days
            var groupedByDay = allResults.GroupBy(x => x._avgTime != null ? x._avgTime.Value.DayOfWeek : DayOfWeek.Sunday);
            foreach (var dayGroup in groupedByDay)
            {
                // Prepare a new list where to put the aggregated results we are about to calculate

                // Grouping by hours
                var groupedByHour = dayGroup.GroupBy(x => x._avgTime != null ? x._avgTime.Value.Hour : 0);
                var tmp = groupedByHour.Select(hourGroup => new AggregatedResult(hourGroup.ToList(), hourGroup.Key)).ToList();
                output.Add(dayGroup.Key, tmp);
            }
            return output;
        }
    }
}