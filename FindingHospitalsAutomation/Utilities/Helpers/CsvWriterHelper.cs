using FindingHospitalsAutomation.Models;
using System.Text;

namespace FindingHospitalsAutomation.Utilities
{
    public static class CsvWriterHelper
    {
        private static readonly string OutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HospitalResults.csv");

        public static void WriteToCsv(List<HospitalInfo> hospitals)
        {
            var sb = new StringBuilder();

            foreach (var hospital in hospitals)
            {
                sb.AppendLine($"Hospital Name: {hospital.Name}");
                sb.AppendLine($"Rating: {hospital.Rating}");
                sb.AppendLine($"Open 24x7: {(hospital.IsOpen24x7 ? "Yes" : "No")}");
                sb.AppendLine($"Has Parking: {(hospital.HasParking ? "Yes" : "No")}");
                sb.AppendLine($"Location: {hospital.Location}");
                sb.AppendLine(); // Blank line for readability
            }

            File.WriteAllText(OutputPath, sb.ToString());
            Console.WriteLine($"📁 CSV written to: {OutputPath}");
        }
    }
}
