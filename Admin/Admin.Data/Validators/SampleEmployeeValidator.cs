using CoreType.DBModels;
using FluentValidation;

namespace Admin.Data.Validators
{
    public class SampleEmployeeValidator : AbstractValidator<SampleEmployee>
    {
        public SampleEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotNull();

            RuleFor(x => x.EmployeeName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50);

            RuleFor(x => x.EmployeeSurname)
                .NotNull()
                .NotEmpty()
                .Length(2, 50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(80)
                .EmailAddress();

            RuleFor(x => x.TaxNumber)
                .NotNull()
                .MaximumLength(10);

            RuleFor(x => x.Password)
                .MaximumLength(64);
        }
    }
}