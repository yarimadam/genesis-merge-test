using CoreData.Validators;
using FluentValidation;
using CoreType.DBModels;

namespace CoreData.Validators
{
    public class CommunicationTemplateValidator : AbstractValidator<CommunicationTemplate>
    {
        public CommunicationTemplateValidator()
        {
            RuleFor(x => x.CommTemplateId)
                .NotNull();

            RuleFor(x => x.CommTemplateName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CommDefinitionId)
                .NotNull();

            RuleFor(x => x.EmailRecipients)
                .MaximumLength(250);

            RuleFor(x => x.EmailCcs)
                .MaximumLength(250);

            RuleFor(x => x.EmailBccs)
                .MaximumLength(250);

            RuleFor(x => x.EmailSubject)
                .MaximumLength(250);

            RuleFor(x => x.EmailSenderName)
                .MaximumLength(100);

            RuleFor(x => x.SmsRecipients)
                .MaximumLength(250);

            RuleFor(x => x.Timezone)
                .MaximumLength(250);

            RuleFor(x => x.RequestType)
                .MaximumLength(1000);

            RuleFor(x => x.ResponseType)
                .MaximumLength(1000);

            RuleFor(x => x.Status)
                .NotNull();

            RuleFor(x => x.TenantId)
                .NotNull();
        }
    }
}