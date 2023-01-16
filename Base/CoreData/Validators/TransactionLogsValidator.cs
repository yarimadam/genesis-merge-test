using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    // TODO Use & edit validator after logging moved to ef core
    public class TransactionLogsValidator : AbstractValidator<TransactionLogs>
    {
        public TransactionLogsValidator()
        {
            RuleFor(x => x.LogId)
                .NotNull();

            RuleFor(x => x.ServiceUrl)
                .NotNull()
                .MaximumLength(254);

            RuleFor(x => x.LogType)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}