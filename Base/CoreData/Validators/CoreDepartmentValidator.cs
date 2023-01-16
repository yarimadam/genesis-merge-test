using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CoreDepartmentValidator : AbstractValidator<CoreDepartment>
    {
        public CoreDepartmentValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}