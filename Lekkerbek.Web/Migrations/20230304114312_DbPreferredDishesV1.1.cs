using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class DbPreferredDishesV11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreferedDishes",
                table: "Customers",
                newName: "PreferredDishId");

            migrationBuilder.CreateTable(
                name: "PreferredDishes",
                columns: table => new
                {
                    PreferredDishId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferredDishes", x => x.PreferredDishId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PreferredDishId",
                table: "Customers",
                column: "PreferredDishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_PreferredDishes_PreferredDishId",
                table: "Customers",
                column: "PreferredDishId",
                principalTable: "PreferredDishes",
                principalColumn: "PreferredDishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_PreferredDishes_PreferredDishId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "PreferredDishes");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PreferredDishId",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "PreferredDishId",
                table: "Customers",
                newName: "PreferedDishes");
        }
    }
}
