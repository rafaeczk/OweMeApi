using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.VerifyCashPayment;

public record VerifyCashPaymentCommand(Guid PaymentId, string Status, string? Note) : IRequest<HandlerResult>;
