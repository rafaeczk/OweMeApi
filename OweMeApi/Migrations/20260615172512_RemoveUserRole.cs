using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRoles_RoleCode",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleCode",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Code", "Label" },
                values: new object[,]
                {
                    { "Admin", "Administrator" },
                    { "Locked", "Zablokowany" },
                    { "Moderator", "Moderator" },
                    { "User", "Użytkownik" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleCode",
                table: "Users",
                column: "RoleCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRoles_RoleCode",
                table: "Users",
                column: "RoleCode",
                principalTable: "UserRoles",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
