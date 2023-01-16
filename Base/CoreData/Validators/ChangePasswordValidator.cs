using CoreType.Models;
using FluentValidation;

namespace CoreData.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(24);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(24);

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage(session => LocalizedMessages.CURRENT_PASSWORD_AND_NEW_PASSWORD_CANNOT_BE_SAME);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}