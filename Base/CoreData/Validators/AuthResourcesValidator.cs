using System;
using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class AuthResourcesValidator : AbstractValidator<AuthResources>
    {
        public AuthResourcesValidator()
        {
            RuleFor(x => x.ParentResourceCode)
                .MaximumLength(250);

            RuleFor(x => x.ResourceCode)
                .NotNull()
                .MaximumLength(250);

            RuleFor(x => x.ResourceCode)
                .NotEqual(x => x.ParentResourceCode, StringComparer.OrdinalIgnoreCase)
                .WithMessage(session => LocalizedMessages.MAIN_RESOURCE_CODE_AND_RESOURCE_CODE_CANNOT_BE_SAME);

            RuleFor(x => x.ResourceName)
                .NotNull()
                .MaximumLength(250);

            RuleFor(x => x.SeoTitle)
                .MaximumLength(75);

            RuleFor(x => x.SeoDescription)
                .MaximumLength(175);

            RuleFor(x => x.SeoKeywords)
                .MaximumLength(75);
        }
    }
}