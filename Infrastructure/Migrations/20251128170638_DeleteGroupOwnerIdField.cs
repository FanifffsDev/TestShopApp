using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestShopApp.Migrations
{
    /// <inheritdoc />
    public partial class DeleteGroupOwnerIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OwnerId",
                table: "Groups",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
