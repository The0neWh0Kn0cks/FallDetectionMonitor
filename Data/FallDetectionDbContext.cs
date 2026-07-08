using FallDetectionMonitor.Models;
using Microsoft.EntityFrameworkCore;

namespace FallDetectionMonitor.Data
{
    public class FallDetectionDbContext : DbContext
    {
        public FallDetectionDbContext(DbContextOptions<FallDetectionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<WearableDevice> WearableDevices { get; set; }

        public DbSet<SensorReading> SensorReadings { get; set; }

        public DbSet<AlertEvent> AlertEvents { get; set; }
    }
}