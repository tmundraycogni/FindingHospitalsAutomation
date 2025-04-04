using System.Text.Json;

namespace FindingHospitalsAutomation.Utilities.Config
{
    public static class TestConfigLoader
    {
        private static readonly string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testConfig.json");
        private static JsonElement _config;

        static TestConfigLoader()
        {
            try
            {
                string json = File.ReadAllText(configFilePath);
                _config = JsonSerializer.Deserialize<JsonElement>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error loading test config: " + ex.Message);
                _config = new JsonElement();
            }
        }

        public static string GetSearchUrl()
        {
            if (_config.TryGetProperty("searchUrl", out JsonElement urlElement))
            {
                return urlElement.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        public static double GetMinRating()
        {
            if (_config.TryGetProperty("minRating", out JsonElement ratingElement))
            {
                return ratingElement.GetDouble();
            }

            return 4.5; // Default fallback
        }
    }
}
