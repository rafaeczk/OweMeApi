namespace OweMeApi.Modules.Debts.Dtos;

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
