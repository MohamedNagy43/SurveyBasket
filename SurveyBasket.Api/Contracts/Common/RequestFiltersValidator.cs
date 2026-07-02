using FluentValidation;

namespace SurveyBasket.Api.Contracts.Common;

public class RequestFiltersValidator : AbstractValidator<RequestFilters>
{
    public RequestFiltersValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(Pagination.MaxPageSize);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);


        RuleFor(x => x.SortDirction)
            .Matches(RegexPatterns.SortDirction)
            .WithMessage("Format accept only ASC and DESC");

    }
}
