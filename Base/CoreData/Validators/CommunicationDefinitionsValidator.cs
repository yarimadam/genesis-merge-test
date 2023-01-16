using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CommunicationDefinitionsValidator : AbstractValidator<CommunicationDefinitions>
    {
        public CommunicationDefinitionsValidator()
        {
            RuleFor(x => x.CommDefinitionId)
                .NotNull();

            RuleFor(x => x.CommDefinitionName)
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.CommDefinitionType)
                .NotNull();

            RuleFor(x => x.EmailSenderName)
                .MaximumLength(100);

            RuleFor(x => x.EmailUsername)
                .MaximumLength(100);

            RuleFor(x => x.EmailPassword)
                .MaximumLength(100);

            RuleFor(x => x.EmailSmtpServer)
                .MaximumLength(100);

            RuleFor(x => x.EmailSenderAccount)
                .MaximumLength(100);

            RuleFor(x => x.EmailPort)
                .MaximumLength(10);

            RuleFor(x => x.SmsCompanyName)
                .MaximumLength(100);

            RuleFor(x => x.SmsProviderCode)
                .MaximumLength(50);

            RuleFor(x => x.SmsCustomerCode)
                .MaximumLength(75);

            RuleFor(x => x.SmsSenderNumber)
                .MaximumLength(75);

            RuleFor(x => x.SmsEndpointUrl)
                .MaximumLength(150);

            RuleFor(x => x.SmsAuthData)
                .MaximumLength(1000);

            RuleFor(x => x.SmsFormData)
                .MaximumLength(500);

            RuleFor(x => x.SmsExpectedStatusCode)
                .MaximumLength(6);

            RuleFor(x => x.SmsExpectedResponse)
                .MaximumLength(250);

            RuleFor(x => x.SmsUsername)
                .MaximumLength(75);

            RuleFor(x => x.SmsPassword)
                .MaximumLength(100);

            RuleFor(x => x.Timezone)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}