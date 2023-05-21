using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChefIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Chefs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chefs_IdentityUserId",
                table: "Chefs",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chefs_AspNetUsers_IdentityUserId",
                table: "Chefs",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chefs_AspNetUsers_IdentityUserId",
                table: "Chefs");

            migrationBuilder.DropIndex(
                name: "IX_Chefs_IdentityUserId",
                table: "Chefs");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Chefs");
        }
    }
}
