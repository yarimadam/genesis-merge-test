using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class AuthActionValidator : AbstractValidator<AuthAction>
    {
        public AuthActionValidator()
        {
            RuleFor(x => x.ActionId)
                .NotNull();

            RuleFor(x => x.ResourceId)
                .NotNull();

            RuleFor(x => x.ActionType)
                .NotNull();

            RuleFor(x => x.OrderIndex)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}