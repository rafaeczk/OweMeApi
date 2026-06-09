using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OweMeApi.Migrations
{
    /// <inheritdoc />
    public partial class EventOrientedDebtLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_CreditorId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_DebtorId",
                table: "Debts");

            migrationBuilder.DropTable(
                name: "LedgerEntries");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "CreditorApproves",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "DebtorApproves",
                table: "Debts");

            migrationBuilder.CreateTable(
                name: "DebtAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtAdjustments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtPayments_Users_PayerId",
                        column: x => x.PayerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtPayments_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtPaymentStatusChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPaymentStatusChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtPaymentStatusChanges_DebtPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "DebtPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LedgerEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DebtId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    InternalReference = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentStatusChangeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerEvents_DebtAdjustments_AdjustmentId",
                        column: x => x.AdjustmentId,
                        principalTable: "DebtAdjustments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEvents_DebtPaymentStatusChanges_PaymentStatusChangeId",
                        column: x => x.PaymentStatusChangeId,
                        principalTable: "DebtPaymentStatusChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEvents_DebtPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "DebtPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEvents_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEvents_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_PayerId",
                table: "DebtPayments",
                column: "PayerId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_ReceiverId",
                table: "DebtPayments",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPaymentStatusChanges_PaymentId",
                table: "DebtPaymentStatusChanges",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_ActorId",
                table: "LedgerEvents",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_AdjustmentId",
                table: "LedgerEvents",
                column: "AdjustmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_DebtId",
                table: "LedgerEvents",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_PaymentId",
                table: "LedgerEvents",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEvents_PaymentStatusChangeId",
                table: "LedgerEvents",
                column: "PaymentStatusChangeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_CreditorId",
                table: "Debts",
                column: "CreditorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_DebtorId",
                table: "Debts",
                column: "DebtorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_CreditorId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_DebtorId",
                table: "Debts");

            migrationBuilder.DropTable(
                name: "LedgerEvents");

            migrationBuilder.DropTable(
                name: "DebtAdjustments");

            migrationBuilder.DropTable(
                name: "DebtPaymentStatusChanges");

            migrationBuilder.DropTable(
                name: "DebtPayments");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Debts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "CreditorApproves",
                table: "Debts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DebtorApproves",
                table: "Debts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DebtId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalReference = table.Column<string>(type: "text", nullable: true),
                    InternalReference = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_Debts_DebtId",
                        column: x => x.DebtId,
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_CreatedByUserId",
                table: "LedgerEntries",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_DebtId",
                table: "LedgerEntries",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_InternalReference",
                table: "LedgerEntries",
                column: "InternalReference",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_CreditorId",
                table: "Debts",
                column: "CreditorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_DebtorId",
                table: "Debts",
                column: "DebtorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
