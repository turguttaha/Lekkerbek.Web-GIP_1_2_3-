using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class deleteTypeOfDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeOfDay",
                table: "RestaurantHolidays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeOfDay",
                table: "RestaurantHolidays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
