namespace OweMeApi.Modules.Debts.Features.GetDebtHistory;

public record DebtHistoryListItemAdjustmentDTO(
    Guid AdjustmentId, 
    decimal 
    Amount, 
    string? Note);

public record DebtHistoryListItemPaymentStatusChangeDTO(
    Guid StatusChangeId,
    string Status,
    string? Note);

public record DebtHistoryListItemPaymentHistoryListItemDTO(
    Guid LedgerEventId,
    string EventType,
    string InternalReference,
    DateTime Timestamp,
    Guid ActorId,
    DebtHistoryListItemPaymentStatusChangeDTO StatusChange);

public record DebtHistoryListItemPaymentDTO(
    Guid PaymentId,
    decimal Amount, 
    Guid PayerId, 
    Guid ReceiverId,
    string Method, 
    string? Note,
    string CurrentStatus,
    List<DebtHistoryListItemPaymentHistoryListItemDTO> StatusChanges);

public record DebtHistoryListItemDTO(
    Guid LedgerEventId,
    string EventType,
    string InternalReference,
    DateTime Timestamp,
    Guid ActorId,
    // DETAILS
    DebtHistoryListItemAdjustmentDTO? Adjustment,
    DebtHistoryListItemPaymentDTO? Payment);
