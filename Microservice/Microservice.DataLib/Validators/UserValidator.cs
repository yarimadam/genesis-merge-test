using CoreData.Validators;
using FluentValidation;
using Microservice.TypeLib.DBModels;

namespace Microservice.DataLib.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            
        }
    }
}