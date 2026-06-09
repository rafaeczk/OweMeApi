using MediatR;
using OweMeApi.Common;
using OweMeApi.Data.Entities.Ledger;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public record VerifyCashPaymentCommand(Guid UserId, Guid LedgerEventId, PaymentStatus Status, string? Note) : IRequest<HandlerResult>;
