namespace OweMeApi.Modules.Debts.Dtos;

public record DebtInformationListItemDTO(
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
