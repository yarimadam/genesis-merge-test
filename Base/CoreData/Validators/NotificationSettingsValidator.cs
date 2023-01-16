using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class NotificationSettingsValidator : AbstractValidator<NotificationSettings>
    {
        public NotificationSettingsValidator()
        {
            RuleFor(x => x.NotificationSettingsId)
                .NotNull();

            RuleFor(x => x.NotificationType)
                .NotNull();

            RuleFor(x => x.Title)
                .MaximumLength(60);

            RuleFor(x => x.Subtitle)
                .MaximumLength(60);

            RuleFor(x => x.Message)
                .MaximumLength(254);

            RuleFor(x => x.AgeRange)
                .MaximumLength(20);

            RuleFor(x => x.Description)
                .MaximumLength(100);

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}