using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class productsv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isInWishlist",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isInWishlist",
                table: "Products",
                type: "bit",
                nullable: true);
        }
    }
}
