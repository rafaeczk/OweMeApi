namespace Application.Modules.Debts.GetDebts;

public record DebtListItemDTO(
    Guid Id,
    string Title,
    string? Description,
    Guid CreditorId,
    Guid DebtorId,
    decimal TotalAmount,
    decimal TotalPayments,
    bool CreditorApproves,
    bool DebtorApproves,
    bool IsSettled
);
