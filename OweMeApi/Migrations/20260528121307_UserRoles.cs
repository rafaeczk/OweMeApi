using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class UserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleCode",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Code", "Label" },
                values: new object[,]
                {
                    { "ADMIN", "Administrator" },
                    { "MODERATOR", "Moderator" },
                    { "USER", "Użytkownik" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleCode",
                table: "Users",
                column: "RoleCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRole_RoleCode",
                table: "Users",
                column: "RoleCode",
                principalTable: "UserRole",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRole_RoleCode",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleCode",
                table: "Users");
        }
    }
}
