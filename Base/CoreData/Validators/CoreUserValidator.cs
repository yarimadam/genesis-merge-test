using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CoreUserValidator : AbstractValidator<CoreUsers>
    {
        public CoreUserValidator()
        {
            RuleFor(x => x.IbanNumber)
                .MinimumLength(20)
                .MaximumLength(32);

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

            RuleFor(x => x.RegistrationNumber)
                .MaximumLength(20);

            RuleFor(x => x.IdentificationNo)
                .MaximumLength(15);

            RuleFor(x => x.Password)
                .MaximumLength(64);

            RuleFor(x => x.TempPassword)
                .MaximumLength(64);

            RuleFor(x => x.Address)
                .MaximumLength(500);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(16);
        }
    }
}