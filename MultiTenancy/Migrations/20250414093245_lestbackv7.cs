using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class lestbackv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "OrderItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "OrderItems");
        }
    }
}
