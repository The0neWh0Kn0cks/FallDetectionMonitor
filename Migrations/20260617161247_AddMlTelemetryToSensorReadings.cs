using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FallDetectionMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddMlTelemetryToSensorReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MlFallScore",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "MlTriggered",
                table: "SensorReadings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MlFallScore",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "MlTriggered",
                table: "SensorReadings");
        }
    }
}
