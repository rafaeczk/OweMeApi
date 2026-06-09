using MediatR;
using OweMeApi.Common;
using OweMeApi.Modules.Debts.Dtos;

namespace OweMeApi.Modules.Debts.Features.GetDebt;

public record GetDebtQuery(Guid UserId, Guid DebtId) : IRequest<HandlerResult<DebtDTO>>;
