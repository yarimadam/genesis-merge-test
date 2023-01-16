using CoreType.DBModels;
using FluentValidation;

namespace Admin.Data.Validators
{
    public class SampleEmployeeTaskValidator : AbstractValidator<SampleEmployeeTask>
    {
        public SampleEmployeeTaskValidator()
        {
        }
    }
}