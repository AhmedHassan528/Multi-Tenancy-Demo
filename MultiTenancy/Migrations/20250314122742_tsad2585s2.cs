using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class tsad2585s2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "PaymentIntentId");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "CartOwner");

            migrationBuilder.RenameColumn(
                name: "ShippingAddressId",
                table: "Orders",
                newName: "CartId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ShippingAddressId",
                table: "Orders",
                newName: "IX_Orders_CartId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderItems",
                newName: "Count");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Carts_CartId",
                table: "Orders",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Carts_CartId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CartOwner",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Orders",
                newName: "ShippingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CartId",
                table: "Orders",
                newName: "IX_Orders_ShippingAddressId");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "OrderItems",
                newName: "Quantity");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
