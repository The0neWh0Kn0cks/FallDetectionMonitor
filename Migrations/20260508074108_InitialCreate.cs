using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FallDetectionMonitor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WearableDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WearableDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WearableDevices_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WearableDeviceId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertEvents_WearableDevices_WearableDeviceId",
                        column: x => x.WearableDeviceId,
                        principalTable: "WearableDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WearableDeviceId = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeartRateBpm = table.Column<int>(type: "int", nullable: false),
                    PressureHpa = table.Column<double>(type: "float", nullable: false),
                    AltitudeM = table.Column<double>(type: "float", nullable: false),
                    AccelX = table.Column<double>(type: "float", nullable: false),
                    AccelY = table.Column<double>(type: "float", nullable: false),
                    AccelZ = table.Column<double>(type: "float", nullable: false),
                    GyroX = table.Column<double>(type: "float", nullable: false),
                    GyroY = table.Column<double>(type: "float", nullable: false),
                    GyroZ = table.Column<double>(type: "float", nullable: false),
                    FallDetected = table.Column<bool>(type: "bit", nullable: false),
                    LedActive = table.Column<bool>(type: "bit", nullable: false),
                    BuzzerActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorReadings_WearableDevices_WearableDeviceId",
                        column: x => x.WearableDeviceId,
                        principalTable: "WearableDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertEvents_WearableDeviceId",
                table: "AlertEvents",
                column: "WearableDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_WearableDeviceId",
                table: "SensorReadings",
                column: "WearableDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_WearableDevices_PatientId",
                table: "WearableDevices",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertEvents");

            migrationBuilder.DropTable(
                name: "SensorReadings");

            migrationBuilder.DropTable(
                name: "WearableDevices");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
