using System;
using System.Collections.Generic;
using System.Linq;

namespace PingExperiment.Entities
{
    /// <summary>
    /// This class' used for aggregating many PingResults. It's similar to what PingResult is for PingResultEntry.
    /// </summary>
    public class AggregatedResult
    {
        // The list of result to aggregate
        private readonly List<PingResult> _results;
        private double? _avg;
        private double? _dev;

        private int Hour { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="results"></param>
        /// <param name="hour"></param>
        public AggregatedResult(List<PingResult> results, int hour)
        {
            _results = results;
            Hour = hour;
            _avg = null;
            _dev = null;
        }

        // ReSharper disable once UnusedMember.Global
        public double GetAvg()
        {
            if (_avg == null)
            {
                _avg = _results.Average(x =>
                {
                    var avg = x.GetAvg();
                    return avg ?? 0;
                });
            }
            return _avg.Value;
        }

        // ReSharper disable once UnusedMember.Global
        public double? GetDev()
        {
            if (_dev == null)
            {
                _dev = Math.Pow(_results.Aggregate(0.0, (acc, x) =>
                {
                    var avg = x.GetAvg();
                    return _avg != null ? (avg != null ? acc + Math.Pow((avg.Value - _avg.Value), 2) : 0) : 0;
                }) / _results.Count, 0.5);
            }
            return _dev.Value;
        }

        // ReSharper disable once UnusedMember.Global
        public int GetCount()
        {
            return _results.Count();
        }

        // Can be used to approssimate the number of "total days" our data covers
        // ReSharper disable once UnusedMember.Global
        public int GetDayCount()
        {
            return _results.Select(x =>
            {
                var dateTime = x.GetAvgTime();
                return dateTime != null
                    ? new DateTime(
                        dateTime.Value.Day +
                        dateTime.Value.Month +
                        dateTime.Value.Year)
                    : new DateTime();
            })
                .Distinct().Count();
        }
    }
}