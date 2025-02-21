using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Application.Currency.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace CurrencyConverter.Application.Tests.Currency.Validators
{
    public class GetLatestRatesQueryValidatorTests
    {
        private readonly GetLatestRatesQueryValidator _validator;

        public GetLatestRatesQueryValidatorTests()
        {
            _validator = new GetLatestRatesQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_BaseCurrency_Is_Empty()
        {
            var query = new GetLatestRatesQuery
            {
                BaseCurrency = ""
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.BaseCurrency);
        }

        [Fact]
        public void Should_Not_Have_Error_When_BaseCurrency_Is_Not_Empty()
        {
            var query = new GetLatestRatesQuery
            {
                BaseCurrency = "USD"
            };

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.BaseCurrency);
        }
    }
}
