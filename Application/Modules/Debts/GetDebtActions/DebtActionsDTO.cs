namespace Application.Modules.Debts.GetDebtActions;

public record DebtApprovementChangeActionDTO(bool Enabled, bool? NextValue);

public record DebtAmountChangeActionDTO(bool Enabled);

public record DebtPaymentActionDTO(bool Enabled);

public record DebtInformationChangeActionDTO(bool Enabled);

public record DebtActionsDTO(
    DebtApprovementChangeActionDTO ApprovementChange,
    DebtAmountChangeActionDTO AmountChange,
    DebtPaymentActionDTO Payment,
    DebtInformationChangeActionDTO InformationChange);
