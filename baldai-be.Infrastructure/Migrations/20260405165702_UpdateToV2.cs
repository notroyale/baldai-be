using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace baldai_be.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dimensions",
                table: "Products",
                newName: "Dimensions_Width");

            migrationBuilder.AddColumn<decimal>(
                name: "WalletBalance",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Dimensions_Height",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Dimensions_Length",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Shipping",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Dimensions_Height",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Dimensions_Length",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Dimensions_Width",
                table: "Products",
                newName: "Dimensions");
        }
    }
}
