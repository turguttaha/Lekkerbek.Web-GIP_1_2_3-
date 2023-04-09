using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class DbCustomerNewProperties_WorkerSchedule_MenuItemNeWProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Customers",
                newName: "Password");

            migrationBuilder.AddColumn<double>(
                name: "BtwNumber",
                table: "MenuItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Sort",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Btw",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "BtwNumber",
                table: "Customers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Customers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmName",
                table: "Customers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkerScheduleId",
                table: "Chefs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkerSchedules",
                columns: table => new
                {
                    WorkerScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChefId = table.Column<int>(type: "int", nullable: false),
                    DayOfTheWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerSchedules", x => x.WorkerScheduleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chefs_WorkerScheduleId",
                table: "Chefs",
                column: "WorkerScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chefs_WorkerSchedules_WorkerScheduleId",
                table: "Chefs",
                column: "WorkerScheduleId",
                principalTable: "WorkerSchedules",
                principalColumn: "WorkerScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chefs_WorkerSchedules_WorkerScheduleId",
                table: "Chefs");

            migrationBuilder.DropTable(
                name: "WorkerSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Chefs_WorkerScheduleId",
                table: "Chefs");

            migrationBuilder.DropColumn(
                name: "BtwNumber",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Sort",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Btw",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BtwNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirmName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "WorkerScheduleId",
                table: "Chefs");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Customers",
                newName: "Address");
        }
    }
}
