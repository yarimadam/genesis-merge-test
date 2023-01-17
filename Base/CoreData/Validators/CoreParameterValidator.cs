using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class CoreParameterValidator : AbstractValidator<CoreParameter>
    {
        public CoreParameterValidator()
        {
            RuleFor(x => x.ParameterId)
                .NotNull();

            RuleFor(x => x.ParentValue)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}