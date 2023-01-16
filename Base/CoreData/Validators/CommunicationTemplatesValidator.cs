using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CommunicationTemplatesValidator : AbstractValidator<CommunicationTemplates>
    {
        private static bool isSmsFieldsEmpty(CommunicationTemplates x) => string.IsNullOrEmpty(x.SmsBody) || string.IsNullOrEmpty(x.SmsRecipients);

        public CommunicationTemplatesValidator()
        {
            RuleFor(x => x.CommTemplateId)
                .NotNull();

            RuleFor(x => x.CommTemplateName)
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.CommDefinitionId)
                .NotNull();

            RuleFor(x => x.EmailRecipients)
                .NotEmpty().When(x => isSmsFieldsEmpty(x) && string.IsNullOrEmpty(x.EmailCcs) && string.IsNullOrEmpty(x.EmailBccs))
                .MaximumLength(250);

            RuleFor(x => x.EmailCcs)
                .NotEmpty().When(x => isSmsFieldsEmpty(x) && string.IsNullOrEmpty(x.EmailRecipients) && string.IsNullOrEmpty(x.EmailBccs))
                .MaximumLength(250);

            RuleFor(x => x.EmailBccs)
                .NotEmpty().When(x => isSmsFieldsEmpty(x) && string.IsNullOrEmpty(x.EmailRecipients) && string.IsNullOrEmpty(x.EmailCcs))
                .MaximumLength(250);

            RuleFor(x => x.EmailSubject)
                .NotEmpty().When(x => isSmsFieldsEmpty(x))
                .MaximumLength(250);

            RuleFor(x => x.EmailSenderName)
                .MaximumLength(100);

            RuleFor(x => x.Timezone)
                .MaximumLength(250);

            RuleFor(x => x.RequestType)
                .MaximumLength(1000);

            RuleFor(x => x.ResponseType)
                .MaximumLength(1000);

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}