using CurrencyConverter.Application.Currency.Queries;
using FluentValidation;

namespace CurrencyConverter.Application.Currency.Validators
{
    public class GetLatestRatesQueryValidator : AbstractValidator<GetLatestRatesQuery>
    {
        public GetLatestRatesQueryValidator()
        {
            RuleFor(x => x.BaseCurrency).NotEmpty();
        }
    }
}
