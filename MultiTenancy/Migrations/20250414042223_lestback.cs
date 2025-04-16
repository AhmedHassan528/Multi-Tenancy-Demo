using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenancy.Migrations
{
    /// <inheritdoc />
    public partial class lestback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "viewCount",
                table: "Products",
                newName: "ViewCount");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Products",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ratingsQuantity",
                table: "Products",
                newName: "RatingsQuantity");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "imageCover",
                table: "Products",
                newName: "ImageCover");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "image",
                table: "Categories",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "image",
                table: "Brands",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "userID",
                table: "Addresses",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "phoneNumber",
                table: "Addresses",
                newName: "PhoneNumber");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "statusMess",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ViewCount",
                table: "Products",
                newName: "viewCount");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Products",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "RatingsQuantity",
                table: "Products",
                newName: "ratingsQuantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "ImageCover",
                table: "Products",
                newName: "imageCover");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Categories",
                newName: "image");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Brands",
                newName: "image");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Addresses",
                newName: "userID");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Addresses",
                newName: "phoneNumber");
        }
    }
}
