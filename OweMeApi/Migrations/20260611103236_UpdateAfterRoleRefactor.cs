using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAfterRoleRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "ADMIN");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "LOCKED");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "MODERATOR");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "USER");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "Admin");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "Locked");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "Moderator");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "User");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Code", "Label" },
                values: new object[,]
                {
                    { "ADMIN", "Administrator" },
                    { "LOCKED", "Zablokowany" },
                    { "MODERATOR", "Moderator" },
                    { "USER", "Użytkownik" }
                });
        }
    }
}
