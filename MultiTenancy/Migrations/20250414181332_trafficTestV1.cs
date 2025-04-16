using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class trafficTestV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "traffics",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    CategoryCount = table.Column<int>(type: "int", nullable: false),
                    BrandCount = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false),
                    ReqCount = table.Column<int>(type: "int", nullable: false),
                    DateNow = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traffics", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "traffics");
        }
    }
}
