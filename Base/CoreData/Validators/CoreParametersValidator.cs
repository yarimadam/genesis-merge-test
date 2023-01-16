using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CoreParametersValidator : AbstractValidator<CoreParameters>
    {
        public CoreParametersValidator()
        {
            RuleFor(x => x.KeyCode)
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.ParentValue)
                .NotNull();

            RuleFor(x => x.Value)
                .NotNull();

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}