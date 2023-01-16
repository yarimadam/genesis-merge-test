using CoreData.Common;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;

namespace CoreData.Validators
{
    public class TenantValidator : AbstractValidator<Tenant>
    {
        public TenantValidator()
        {
            RuleFor(x => x.TenantName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(150);

            RuleFor(x => x.ParentTenantId)
                .NotEqual(x => x.TenantId)
                .When(x => x.ParentTenantId > 0); // if a parent selected

            RuleFor(x => x.TenantType)
                .NotNull();

            RuleFor(x => x.TenantType)
                .NotEqual(x => (int) TenantType.SystemOwner) // Only SystemOwner can assign a new SystemOwner
                .When((x) => SessionAccessor.GetSession().CurrentUser.TenantType != (int) TenantType.SystemOwner)
                .WithMessage(session => LocalizedMessages.YOU_CANNOT_ASSIGN_SYSTEM_OWNER_TENANTTYPE_MESSAGE);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.TaxNumber)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}