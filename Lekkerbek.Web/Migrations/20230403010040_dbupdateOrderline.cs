using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class dbupdateOrderline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_MenuItems_DishID",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CustomerID",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameColumn(
                name: "DishID",
                table: "OrderLines",
                newName: "MenuItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLines_DishID",
                table: "OrderLines",
                newName: "IX_OrderLines_MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_MenuItems_MenuItemId",
                table: "OrderLines",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_MenuItems_MenuItemId",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Orders",
                newName: "CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                newName: "IX_Orders_CustomerID");

            migrationBuilder.RenameColumn(
                name: "MenuItemId",
                table: "OrderLines",
                newName: "DishID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderLines_MenuItemId",
                table: "OrderLines",
                newName: "IX_OrderLines_DishID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_MenuItems_DishID",
                table: "OrderLines",
                column: "DishID",
                principalTable: "MenuItems",
                principalColumn: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerID",
                table: "Orders",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }
    }
}
