using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class AuthUserRightValidator : AbstractValidator<AuthUserRight>
    {
        public AuthUserRightValidator()
        {
            RuleFor(x => x.RightId)
                .NotNull();

            RuleFor(x => x.ActionId)
                .NotNull();

            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.RecordType)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}