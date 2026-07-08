using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FallDetectionMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddNextOfKinAndEmergencyActionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasNextOfKin",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmergencyServicesCalledAt",
                table: "AlertEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextOfKinCalledAt",
                table: "AlertEvents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNextOfKin",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "EmergencyServicesCalledAt",
                table: "AlertEvents");

            migrationBuilder.DropColumn(
                name: "NextOfKinCalledAt",
                table: "AlertEvents");
        }
    }
}
