using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class SampleEmployeeTaskValidator : AbstractValidator<SampleEmployeeTask>
    {
        public SampleEmployeeTaskValidator()
        {
            RuleFor(x => x.EmployeeTaskId)
                .NotNull();

            RuleFor(x => x.EmployeeId)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}