using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class LockedUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Code", "Label" },
                values: new object[] { "LOCKED", "Zablokowany" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Code",
                keyValue: "LOCKED");
        }
    }
}
