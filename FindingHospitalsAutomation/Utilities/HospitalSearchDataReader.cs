using Newtonsoft.Json;
using System.IO;

namespace FindingHospitalsAutomation.Utilities
{
    public class HospitalSearchData
    {
        public string searchUrl { get; set; } = string.Empty; // fix CS8618
        public double minRating { get; set; }
    }

    public static class HospitalSearchDataReader
    {
        public static HospitalSearchData LoadData()
        {
            string json = File.ReadAllText("hospitalSearch.json");
            var data = JsonConvert.DeserializeObject<HospitalSearchData>(json);

            if (data == null)
                throw new Exception("❌ Failed to load hospital search data from JSON.");

            return data;
        }
    }
}
