using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OweMeApi.Data.Entities.Ledger;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    Cash, Transfer
}

public class DebtPayment
{
    [Key]
    public Guid Id { get; set; }

    public LedgerEvent LedgerEvent { get; set; } = null!;

    public ICollection<DebtPaymentStatusChange> StatusChanges { get; set; } = [];

    public decimal Amount { get; set; }

    public Guid PayerId { get; set; }
    public User Payer { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;

    public PaymentMethod Method { get; set; }

    public string? Note { get; set; }
}
