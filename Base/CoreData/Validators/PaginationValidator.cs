using CoreType.Types;
using FluentValidation;

namespace CoreData.Validators
{
    public class PaginationValidator<T> : AbstractValidator<RequestWithPagination<T>> where T : class, new()
    {
        public PaginationValidator()
        {
            RuleFor(x => x.Pagination.CurrentPage)
                .GreaterThan(0);

            RuleFor(x => x.Pagination.MaxRowsPerPage)
                .GreaterThan(0);

            RuleFor(x => x.Pagination.RowOffset)
                .GreaterThanOrEqualTo(0);
        }
    }
}