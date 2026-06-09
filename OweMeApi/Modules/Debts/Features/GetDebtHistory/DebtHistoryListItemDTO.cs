using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.GetDebtHistory;

public record DebtHistoryListItemAdjustmentDTO(
    Guid AdjustmentId, 
    decimal 
    Amount, 
    string? Note);

public record DebtHistoryListItemPaymentStatusChangeDTO(
    Guid StatusChangeId,
    PaymentStatus Status,
    string? Note);

public record DebtHistoryListItemPaymentHistoryListItemDTO(
    Guid LedgerEventId,
    LedgerEventType EventType,
    string InternalReference,
    DateTime Timestamp,
    Guid ActorId,
    DebtHistoryListItemPaymentStatusChangeDTO StatusChange);

public record DebtHistoryListItemPaymentDTO(
    Guid PaymentId,
    decimal Amount, 
    Guid PayerId, 
    Guid ReceiverId,
    PaymentMethod Method, 
    string? Note,
    PaymentStatus CurrentStatus,
    List<DebtHistoryListItemPaymentHistoryListItemDTO> StatusChanges);

public record DebtHistoryListItemDTO(
    Guid LedgerEventId,
    LedgerEventType EventType,
    string InternalReference,
    DateTime Timestamp,
    Guid ActorId,
    // DETAILS
    DebtHistoryListItemAdjustmentDTO? Adjustment,
    DebtHistoryListItemPaymentDTO? Payment);
