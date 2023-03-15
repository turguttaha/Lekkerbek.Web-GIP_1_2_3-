using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class DbTimeSlot_EndTimeSlot_IsAvailable_Removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTimeSlot",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "TimeSlots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimeSlot",
                table: "TimeSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "TimeSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
