namespace Application.Modules.Debts.GetDebt;

public record DebtDTO(
    Guid Id,
    string Title,
    string? Description,
    Guid CreditorId,
    Guid DebtorId,
    bool YouAreCreditor,
    bool YouAreDebtor,
    DateTime CreatedAt,
    decimal TotalAmount,
    decimal TotalPayments,
    bool CreditorApproves,
    bool DebtorApproves,
    bool IsSettled
);
