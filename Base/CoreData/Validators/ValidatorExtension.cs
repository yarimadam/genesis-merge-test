using System;
using CoreData.CacheManager;
using CoreData.Common;
using CoreType.Types;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace CoreData.Validators
{
    public static class DefaultValidatorExtensions // : Object
    {
        public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance) //where T : Object
        {
            var session = SessionAccessor.GetSession();

            foreach (var propertyRule in (AbstractValidator<T>) validator)
            {
                foreach (var validatorItem in propertyRule.Validators)
                {
                    if (validatorItem.Options.ErrorMessageSource is LanguageStringSource)
                    {
                        var validatorName = validatorItem.GetType().Name;
                        var message = DistributedCache.GetParam(validatorName);

                        if (!string.IsNullOrEmpty(message) && !message.Equals(validatorName))
                            validatorItem.Options.ErrorMessageSource = new StaticStringSource(message);
                    }

//                    else if (validatorItem.Options.ErrorMessageSource is LazyStringSource asd)
//                    {
//                        validatorItem.Options.ErrorMessageSource  =new LazyStringSource(ctx => messageTemplate((T) ctx,session));
//                    }
                }
            }

            var context = new ValidationContext<T>(instance);
            context.RootContextData["Session"] = session;

            ValidationResult validationResult = validator.Validate(context);
            if (validationResult.IsValid == false)
                throw new ValidationException(validationResult.Errors);
        }
    }

    public static class DefaultValidatorOptions // : Object
    {
        public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<SessionContext, string> messageTemplate)
        {
            return WithLocalizedMessage(rule, (session, entity) => messageTemplate(session));
        }

        public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<SessionContext, T, string> messageTemplate)
        {
            return WithLocalizedMessage(rule, messageTemplate);
        }

        public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<SessionContext, string> messageTemplate)
        {
            return WithLocalizedMessage(rule, (session, entity) => messageTemplate(session));
        }

        public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<SessionContext, T, string> messageTemplate)
        {
            return rule.Configure(config =>
                config.CurrentValidator.Options.ErrorMessageSource = (IStringSource) new SessionStringSource((ctx, session) => messageTemplate(session, (T) ctx.InstanceToValidate)));
        }
    }

    public class SessionStringSource : IStringSource
    {
        private readonly Func<ICommonContext, SessionContext, string> _stringProvider;

        public SessionStringSource(Func<ICommonContext, SessionContext, string> stringProvider)
        {
            _stringProvider = stringProvider;
        }

        public string GetString(ICommonContext context)
        {
            try
            {
                var rootContextData = (context as PropertyValidatorContext)?.ParentContext?.RootContextData;

                SessionContext session = null;
                if (rootContextData != null && rootContextData.ContainsKey("Session"))
                    session = rootContextData["Session"] as SessionContext;

                return _stringProvider(context, session);
            }
            catch (NullReferenceException ex)
            {
                throw new FluentValidationMessageFormatException("Could not build error message- the message makes use of properties from the containing object, but the containing object was null.",
                    ex);
            }
        }
    }
}