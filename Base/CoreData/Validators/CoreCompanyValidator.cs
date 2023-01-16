using CoreType.DBModels;
using FluentValidation;

namespace CoreData.Validators
{
    public class CoreCompanyValidator : AbstractValidator<CoreCompany>
    {
        public CoreCompanyValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotNull()
                .MaximumLength(150);

            RuleFor(x => x.CompanyLegalTitle)
                .MaximumLength(150);

            RuleFor(x => x.TaxOffice)
                .MaximumLength(50);

            RuleFor(x => x.TaxNumber)
                .MaximumLength(20);

            RuleFor(x => x.BillingAddress)
                .MaximumLength(250);

            RuleFor(x => x.ContactPerson)
                .MaximumLength(75);

            RuleFor(x => x.ContactPersonTitle)
                .MaximumLength(50);

            RuleFor(x => x.ContactPersonTelephone)
                .MaximumLength(20);

            RuleFor(x => x.ContactPersonEmail)
                .EmailAddress()
                .MaximumLength(75);

            RuleFor(x => x.Address)
                .MaximumLength(250);

            RuleFor(x => x.Telephone)
                .MaximumLength(20);

            RuleFor(x => x.Email)
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Website)
                .MaximumLength(100);

            RuleFor(x => x.Status)
                .NotNull();
        }
    }
}