using Domain.Common;

namespace Domain.Entities;

public class LedgerEvent : BaseAuditableEntity
{
    public Guid DebtId { get; set; }
    public Debt Debt { get; set; } = null!;
    public required string EventType { get; set; }
    public required string InternalReference { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // EVENT DETAILS

    public Guid? AdjustmentId { get; set; }
    public DebtAdjustment? Adjustment { get; set; }

    public Guid? PaymentId { get; set; }
    public DebtPayment? Payment { get; set; }

    public Guid? PaymentStatusChangeId { get; set; }
    public DebtPaymentStatusChange? PaymentStatusChange { get; set; }

    // METHODS

    public static string GenReferenceNumber => $"REF-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
