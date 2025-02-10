using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class productsv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "productID",
                table: "WishLists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productID",
                table: "WishLists");
        }
    }
}
