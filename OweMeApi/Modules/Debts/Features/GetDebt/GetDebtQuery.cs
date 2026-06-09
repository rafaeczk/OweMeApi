using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.GetDebt;

public record GetDebtQuery(Guid DebtId) : IRequest<HandlerResult<DebtDTO>>;
