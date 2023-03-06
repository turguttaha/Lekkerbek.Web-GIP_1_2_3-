using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lekkerbek.Web.Migrations
{
    /// <inheritdoc />
    public partial class DbRelation_OrderLine_TimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TimeSlots_TimeSlotID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TimeSlotID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TimeSlotID",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotID",
                table: "OrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_TimeSlotID",
                table: "OrderLines",
                column: "TimeSlotID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_TimeSlots_TimeSlotID",
                table: "OrderLines",
                column: "TimeSlotID",
                principalTable: "TimeSlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_TimeSlots_TimeSlotID",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_TimeSlotID",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "TimeSlotID",
                table: "OrderLines");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotID",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TimeSlotID",
                table: "Orders",
                column: "TimeSlotID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TimeSlots_TimeSlotID",
                table: "Orders",
                column: "TimeSlotID",
                principalTable: "TimeSlots",
                principalColumn: "Id");
        }
    }
}
