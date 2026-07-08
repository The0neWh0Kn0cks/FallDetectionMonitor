namespace FallDetectionMonitor.Models
{
    public class AlertEvent
    {
        public int Id { get; set; }

        public int WearableDeviceId { get; set; }

        public WearableDevice? WearableDevice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string AlertType { get; set; } = "";

        public string Message { get; set; } = "";

        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedAt { get; set; }

        public DateTime? EmergencyServicesCalledAt { get; set; }

        public DateTime? NextOfKinCalledAt { get; set; }

        public string ResolutionType { get; set; } = "";

        public string ResetReason { get; set; } = "";
    }
}