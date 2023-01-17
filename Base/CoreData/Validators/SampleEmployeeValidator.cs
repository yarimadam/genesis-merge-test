using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class SampleEmployeeValidator : AbstractValidator<SampleEmployee>
    {
        public SampleEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotNull();

            RuleFor(x => x.EmployeeName)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.EmployeeSurname)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.CompanyId)
                .NotNull();

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(80);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(64);

            RuleFor(x => x.TaxNumber)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}