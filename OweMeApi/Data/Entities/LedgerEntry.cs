using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OweMeApi.Data.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    Payment, Adjustment
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Pending, Confirmed, Rejected
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    Cash, Transfer, Adjustment
}

public class LedgerEntry
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string InternalReference { get; set; }

    public string? ExternalReference { get; set; }

    public Guid DebtId { get; set; }
    public Debt Debt { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TransactionType TransactionType { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    // METHODS

    public static string GenReferenceNumber => $"REF-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
