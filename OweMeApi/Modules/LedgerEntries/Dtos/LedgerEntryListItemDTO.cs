using OweMeApi.Data.Entities;

namespace OweMeApi.Modules.LedgerEntries.Dtos;

public record LedgerEntryListItemDTO(
    Guid EntryId,
    string InternalReference,
    string? ExternalReference,
    decimal Amount,
    decimal CurrentDebtAmount,
    string? Note,
    TransactionType TransactionType,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    Guid CreatedByUserId,
    DateTime CreatedAt
);
