using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace FindingHospitalsAutomation.Utilities.Config
{
    public static class PageUrlConfig
    {
        private static readonly Dictionary<string, string> urls;

        static PageUrlConfig()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pageUrls.json");
            urls = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
        }

        public static string GetUrl(string key)
        {
            if (urls.ContainsKey(key))
                return urls[key];
            throw new Exception($"URL key '{key}' not found in pageUrls.json");
        }
    }
}
