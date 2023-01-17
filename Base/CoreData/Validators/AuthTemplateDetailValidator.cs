using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class AuthTemplateDetailValidator : AbstractValidator<AuthTemplateDetail>
    {
        public AuthTemplateDetailValidator()
        {
            RuleFor(x => x.AuthTemplateDetailId)
                .NotNull();

            RuleFor(x => x.AuthTemplateId)
                .NotNull();

            RuleFor(x => x.ResourceId)
                .NotNull();

            RuleFor(x => x.ActionId)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}