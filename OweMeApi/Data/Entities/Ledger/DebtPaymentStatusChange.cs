using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OweMeApi.Data.Entities.Ledger;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Success, Failure, Pending
}

public class DebtPaymentStatusChange
{
    [Key]
    public Guid Id { get; set; }

    public LedgerEvent LedgerEvent { get; set; } = null!;

    public Guid PaymentId { get; set; }
    public DebtPayment Payment { get; set; } = null!;

    public PaymentStatus Status { get; set; }

    public string? Note { get; set; }
}
