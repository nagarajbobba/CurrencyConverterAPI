using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Application.Currency.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace CurrencyConverter.Application.Tests.Currency.Validators
{
    public class GetConversionHistoryQueryValidatorTests
    {
        private readonly GetConversionHistoryQueryValidator _validator;

        public GetConversionHistoryQueryValidatorTests()
        {
            _validator = new GetConversionHistoryQueryValidator();
        }


        [Fact]
        public void Should_Have_Error_When_BaseCurrency_Is_Empty()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "",
                    StartDate = "2023-01-01",
                    EndDate = "2023-01-31",
                    Page = 1,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.BaseCurrency);
        }

        [Fact]
        public void Should_Have_Error_When_StartDate_Is_Empty()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "",
                    EndDate = "2023-01-31",
                    Page = 1,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.StartDate);
        }

        [Fact]
        public void Should_Have_Error_When_EndDate_Is_Empty()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2023-01-01",
                    EndDate = "",
                    Page = 1,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.EndDate);
        }

        [Fact]
        public void Should_Have_Error_When_Page_Is_Less_Than_Or_Equal_To_Zero()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2023-01-01",
                    EndDate = "2023-01-31",
                    Page = 0,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.Page);
        }

        [Fact]
        public void Should_Have_Error_When_PageSize_Is_Less_Than_Or_Equal_To_Zero()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2023-01-01",
                    EndDate = "2023-01-31",
                    Page = 1,
                    PageSize = 0
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.PageSize);
        }

        [Fact]
        public void Should_Have_Error_When_StartDate_Is_Not_Less_Than_EndDate()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2023-02-01",
                    EndDate = "2023-01-31",
                    Page = 1,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.HistoryRequest.StartDate)
                  .WithErrorMessage("Start date must be before end date.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Request()
        {
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2023-01-01",
                    EndDate = "2023-01-31",
                    Page = 1,
                    PageSize = 10
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
