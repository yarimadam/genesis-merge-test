using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class AuthTemplateValidator : AbstractValidator<AuthTemplate>
    {
        public AuthTemplateValidator()
        {
            RuleFor(x => x.AuthTemplateId)
                .NotNull();

            RuleFor(x => x.TemplateName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.TemplateType)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.IsDefault)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}