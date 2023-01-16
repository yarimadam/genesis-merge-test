using CoreType.Models;
using FluentValidation;

namespace CoreData.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<LoggedInUser>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}