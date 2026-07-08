namespace FallDetectionMonitor.Models
{
    public class SensorReading
    {
        public int Id { get; set; }

        public int WearableDeviceId { get; set; }

        public WearableDevice? WearableDevice { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;

        public int HeartRateBpm { get; set; }

        public double PressureHpa { get; set; }

        public double AltitudeM { get; set; }

        public double AccelX { get; set; }

        public double AccelY { get; set; }

        public double AccelZ { get; set; }

        public double GyroX { get; set; }

        public double GyroY { get; set; }

        public double GyroZ { get; set; }

        public bool FallDetected { get; set; }

        public bool LedActive { get; set; }

        public bool BuzzerActive { get; set; }

        public double MlFallScore { get; set; }

        public bool MlTriggered { get; set; }
    }
}