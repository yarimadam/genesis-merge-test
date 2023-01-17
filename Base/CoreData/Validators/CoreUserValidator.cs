using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class CoreUserValidator : AbstractValidator<CoreUser>
    {
        public CoreUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.IbanNumber)
                .MaximumLength(33);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Surname)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .MaximumLength(80);

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
                .MaximumLength(254);

            RuleFor(x => x.ShouldChangePassword)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}