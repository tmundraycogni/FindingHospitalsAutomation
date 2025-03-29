using FindingHospitalsAutomation.Models;
using System.Globalization;
using System.Text;

namespace FindingHospitalsAutomation.Utilities.Csv
{
    public static class CsvWriterHelper
    {
        private static readonly string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hospital_results.csv");

        public static void WriteHospitalsToCsv(List<HospitalInfo> hospitals)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Name,Rating,IsOpen24x7,HasParking,Location");

            foreach (var hospital in hospitals)
            {
                var line = string.Format(CultureInfo.InvariantCulture,
                    "\"{0}\",{1},{2},{3},\"{4}\"",
                    hospital.Name.Replace("\"", "\"\""),
                    hospital.Rating,
                    hospital.IsOpen24x7,
                    hospital.HasParking,
                    hospital.Location.Replace("\"", "\"\"")
                );

                csv.AppendLine(line);
            }

            File.WriteAllText(csvFilePath, csv.ToString());
        }

        public static string GetCsvFilePath()
        {
            return csvFilePath;
        }
    }
}
