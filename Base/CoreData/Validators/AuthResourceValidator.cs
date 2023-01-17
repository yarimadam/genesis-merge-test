using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class AuthResourceValidator : AbstractValidator<AuthResource>
    {
        public AuthResourceValidator()
        {
            RuleFor(x => x.ResourceId)
                .NotNull();

            RuleFor(x => x.ParentResourceCode)
                .MaximumLength(250);

            RuleFor(x => x.ResourceCode)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.ResourceName)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.ResourceType)
                .NotNull();

            RuleFor(x => x.OrderIndex)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.SeoTitle)
                .MaximumLength(75);

            RuleFor(x => x.SeoDescription)
                .MaximumLength(175);

            RuleFor(x => x.SeoKeywords)
                .MaximumLength(75);

            RuleFor(x => x.TableName)
                .MaximumLength(100);

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}