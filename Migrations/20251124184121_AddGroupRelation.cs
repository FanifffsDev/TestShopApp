using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestShopApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Group",
                table: "Users",
                newName: "HeadmenOf");

            migrationBuilder.AddColumn<string>(
                name: "GroupNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Number = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Number);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupNumber",
                table: "Users",
                column: "GroupNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupNumber",
                table: "Users",
                column: "GroupNumber",
                principalTable: "Groups",
                principalColumn: "Number",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupNumber",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "HeadmenOf",
                table: "Users",
                newName: "Group");
        }
    }
}
