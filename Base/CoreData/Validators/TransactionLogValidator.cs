using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class TransactionLogValidator : AbstractValidator<TransactionLog>
    {
        public TransactionLogValidator()
        {
            RuleFor(x => x.LogId)
                .NotNull();

            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.ServiceUrl)
                .NotEmpty()
                .MaximumLength(254);

            RuleFor(x => x.LogType)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}