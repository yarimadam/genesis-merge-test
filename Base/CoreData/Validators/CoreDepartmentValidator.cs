using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class CoreDepartmentValidator : AbstractValidator<CoreDepartment>
    {
        public CoreDepartmentValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotNull();

            RuleFor(x => x.DepartmentName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}