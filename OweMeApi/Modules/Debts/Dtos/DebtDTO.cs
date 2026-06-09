namespace OweMeApi.Modules.Debts.Dtos;

public record DebtDTO(
    Guid Id,
    string Title,
    string? Description,
    Guid CreditorId,
    Guid DebtorId,
    decimal TotalAmount,
    decimal TotalPayments,
    DebtSummaryDTO Summary,
    bool CreditorApproves,
    bool DebtorApproves,
    bool IsSettled
);
