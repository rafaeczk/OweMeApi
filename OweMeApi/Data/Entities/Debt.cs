using System.ComponentModel.DataAnnotations;

namespace OweMeApi.Data.Entities;

public class Debt
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; set; }

    public Guid CreditorId { get; set; }
    public User Creditor { get; set; } = null!;

    public Guid DebtorId { get; set; }
    public User Debtor { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "The debt amount must be greater than zero.")]
    public required decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool CreditorApproves { get; private set; } = false;
    public bool DebtorApproves { get; private set; } = false;
    public bool IsSettled => CreditorApproves && DebtorApproves;

    public void ToggleCreditorApproval(bool approved)
    {
        if (IsSettled) throw new InvalidOperationException("The debt is already settled.");

        CreditorApproves = approved;
    }

    public void ToggleDebtorApproval(bool approved)
    {
        if (IsSettled) throw new InvalidOperationException("The debt is already settled.");

        DebtorApproves = approved;
    }
}
