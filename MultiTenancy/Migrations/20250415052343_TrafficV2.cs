using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class TrafficV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReqCount",
                table: "traffics");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "traffics",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "RequestDates",
                table: "traffics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestDates",
                table: "traffics");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "traffics",
                newName: "id");

            migrationBuilder.AddColumn<int>(
                name: "ReqCount",
                table: "traffics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
