namespace FindingHospitalsAutomation.Models
{
    public class HospitalInfo
    {
        public string Name { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsOpen24x7 { get; set; }
        public bool HasParking { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
