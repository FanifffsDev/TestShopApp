using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestShopApp.Migrations
{
    /// <inheritdoc />
    public partial class GroupUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeadmenOf",
                table: "Users",
                newName: "HeadmanOf");

            migrationBuilder.CreateIndex(
                name: "IX_Users_HeadmanOf",
                table: "Users",
                column: "HeadmanOf",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_HeadmanOf",
                table: "Users",
                column: "HeadmanOf",
                principalTable: "Groups",
                principalColumn: "Number",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_HeadmanOf",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HeadmanOf",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "HeadmanOf",
                table: "Users",
                newName: "HeadmenOf");
        }
    }
}
