using System.ComponentModel.DataAnnotations;

namespace FallDetectionMonitor.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public int Age { get; set; }

        public string Location { get; set; } = "Home";

        public string MedicalHistory { get; set; } = "Not recorded yet";

        public bool HasNextOfKin { get; set; } = true;

        public string EmergencyContactName { get; set; } = "";

        public string EmergencyContactPhone { get; set; } = "";

        public List<WearableDevice> WearableDevices { get; set; } = new();
    }
}