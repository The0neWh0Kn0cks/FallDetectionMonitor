using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FallDetectionMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertResolutionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetReason",
                table: "AlertEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolutionType",
                table: "AlertEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetReason",
                table: "AlertEvents");

            migrationBuilder.DropColumn(
                name: "ResolutionType",
                table: "AlertEvents");
        }
    }
}
