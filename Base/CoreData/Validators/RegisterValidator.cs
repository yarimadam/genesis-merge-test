using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class RegisterValidator : AbstractValidator<CoreUsers>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(30);

            RuleFor(x => x.Surname)
                .MinimumLength(2)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(50)
                .EmailAddress();

            RuleFor(x => x.Password)
                .MaximumLength(64);
        }
    }
}