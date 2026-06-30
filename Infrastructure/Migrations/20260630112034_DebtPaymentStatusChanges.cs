using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DebtPaymentStatusChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEvents_DebtPayments_DebtPaymentId",
                table: "LedgerEvents");

            migrationBuilder.DropIndex(
                name: "IX_LedgerEvents_DebtPaymentId",
                table: "LedgerEvents");

            migrationBuilder.DropColumn(
                name: "DebtPaymentId",
                table: "LedgerEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DebtPaymentId",
                table: "LedgerEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_DebtPaymentId",
                table: "LedgerEvents",
                column: "DebtPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEvents_DebtPayments_DebtPaymentId",
                table: "LedgerEvents",
                column: "DebtPaymentId",
                principalTable: "DebtPayments",
                principalColumn: "Id");
        }
    }
}
