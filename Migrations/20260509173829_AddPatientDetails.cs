using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FallDetectionMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistory",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalHistory",
                table: "Patients");
        }
    }
}
