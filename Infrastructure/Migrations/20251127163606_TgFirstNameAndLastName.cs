using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestShopApp.Migrations
{
    /// <inheritdoc />
    public partial class TgFirstNameAndLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TgFirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TgLastName",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TgFirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TgLastName",
                table: "Users");
        }
    }
}
