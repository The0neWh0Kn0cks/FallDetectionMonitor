using System.ComponentModel.DataAnnotations;

namespace FallDetectionMonitor.Models
{
    public class WearableDevice
    {
        public int Id { get; set; }

        [Required]
        public string DeviceName { get; set; } = "";

        public string DeviceCode { get; set; } = "";

        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        public List<SensorReading> SensorReadings { get; set; } = new();

        public List<AlertEvent> AlertEvents { get; set; } = new();
    }
}