using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OweMeApi.Data.Entities.Ledger;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LedgerEventType
{
    Adjustment, 
    Payment, PaymentStatusChange,
    CreditorDebtApprovement, CreditorDebtDisapprovement, DebtorDebtApprovement, DebtorDebtDisapprovement,
    DebtSettlement
}

public class LedgerEvent
{
    [Key]
    public Guid Id { get; set; }

    public Guid DebtId { get; set; }
    public Debt Debt { get; set; } = null!;

    public LedgerEventType EventType { get; set; }

    public required string InternalReference { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Guid ActorId { get; set; }
    public User? Actor { get; set; }

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
