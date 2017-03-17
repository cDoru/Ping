using System;
using System.Text.RegularExpressions;

namespace PingExperiment
{
    class RegexHelper
    {
        private const string _urlRegex =
            @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";

        private Regex UrlRegex { get; set; }

        public RegexHelper()
        {
            UrlRegex = new Regex(_urlRegex, RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        }

        public bool IsValid(string url)
        {
            return UrlRegex.IsMatch(url);
        }
    }
}