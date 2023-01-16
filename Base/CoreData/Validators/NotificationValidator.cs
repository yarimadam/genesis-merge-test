using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class NotificationValidator : AbstractValidator<Notification>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotNull();

            RuleFor(x => x.NotificationSettingsId)
                .NotNull();

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}