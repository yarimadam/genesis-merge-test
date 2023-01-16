using CoreType.Models;
using FluentValidation;

namespace CoreData.Validators
{
    public class ResetPasswordValidator : AbstractValidator<LoggedInUser>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.ForgotPasswordKey)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(24);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}