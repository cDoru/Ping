using System;

namespace PingExperiment.Interfaces
{
    public interface IConfigurationProvider
    {
        void Ingest<T>(Action<T> setter, string key);
    }
}
