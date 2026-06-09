using MediatR;
using OweMeApi.Common;

namespace OweMeApi.Modules.Debts.Features.EditDebtInformation;

public record EditDebtInformationCommand(Guid UserId, Guid DebtId, string Title, string? Description) : IRequest<HandlerResult>;
