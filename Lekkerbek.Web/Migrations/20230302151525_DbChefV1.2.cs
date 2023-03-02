using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class DbChefV12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChefId",
                table: "TimeSlot",
                newName: "Chef2Id");

            migrationBuilder.AddColumn<int>(
                name: "Chef1Id",
                table: "TimeSlot",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chef1Id",
                table: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "Chef2Id",
                table: "TimeSlot",
                newName: "ChefId");
        }
    }
}
