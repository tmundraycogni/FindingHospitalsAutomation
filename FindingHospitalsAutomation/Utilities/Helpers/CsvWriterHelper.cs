using System.Globalization;
using System.Text;
using FindingHospitalsAutomation.Models;

namespace FindingHospitalsAutomation.Utilities.Csv
{
    public static class CsvWriterHelper
    {
        public static void WriteHospitalsToCsv(List<HospitalInfo> hospitals)
        {
            string filePath = "HospitalResults.csv";

            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Name,Rating,Open 24x7,Parking,Location");

                foreach (var hospital in hospitals)
                {
                    string line = string.Format(CultureInfo.InvariantCulture,
                        "\"{0}\",{1},{2},{3},\"{4}\"",
                        hospital.Name,
                        hospital.Rating,
                        hospital.IsOpen24x7 ? "Yes" : "No",
                        hospital.HasParking ? "Yes" : "No",
                        hospital.Location.Replace(",", " ")
                    );
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine($"✅ CSV saved: {Path.GetFullPath(filePath)}");
        }
    }
}
