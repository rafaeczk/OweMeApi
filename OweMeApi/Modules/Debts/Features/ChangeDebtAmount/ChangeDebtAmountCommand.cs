using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.ChangeDebtAmount;

public record ChangeDebtAmountCommand(Guid DebtId, decimal Amount, string Note) : IRequest<HandlerResult>;
