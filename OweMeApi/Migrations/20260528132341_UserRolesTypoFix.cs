using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class UserRolesTypoFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRole_RoleCode",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "UserRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRoles_RoleCode",
                table: "Users",
                column: "RoleCode",
                principalTable: "UserRoles",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRoles_RoleCode",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRole_RoleCode",
                table: "Users",
                column: "RoleCode",
                principalTable: "UserRole",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
