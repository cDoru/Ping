using System;
using System.Configuration;
using PingExperiment.Interfaces;
using ConfigurationException = PingExperiment.Exceptions.ConfigurationException;

namespace PingExperiment.Implementations
{
    public class AppConfigConfigurationProvider : IConfigurationProvider
    {
        private const string ConfigurationExceptionMessage =
            "{0} key was not found in the configuration (loaded with app config configuration provider)";

        public void Ingest<T>(Action<T> setter, string key)
        {
            var configurationValue = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(configurationValue))
            {
                throw new ConfigurationException(ConfigurationExceptionMessage);
            }

            var value = GetValue<T>(configurationValue);
            setter(value);
        }

        private static T GetValue<T>(String value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}