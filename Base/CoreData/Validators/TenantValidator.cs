using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class TenantValidator : AbstractValidator<Tenant>
    {
        public TenantValidator()
        {
            RuleFor(x => x.TenantId)
                .NotNull();

            RuleFor(x => x.TenantType)
                .NotNull();

            RuleFor(x => x.TaxOffice)
                .MaximumLength(50);

            RuleFor(x => x.TaxNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Note)
                .MaximumLength(500);

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.IsDefault)
                .NotNull();
        }
    }
}