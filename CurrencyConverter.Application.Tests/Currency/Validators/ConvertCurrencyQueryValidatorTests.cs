using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Application.Currency.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace CurrencyConverter.Application.Tests.Currency.Validators
{
    public class ConvertCurrencyQueryValidatorTests
    {
        private readonly ConvertCurrencyQueryValidator _validator;
        private readonly Mock<IExchangeSettings> _exchangeSettingsMock;

        public ConvertCurrencyQueryValidatorTests()
        {
            _exchangeSettingsMock = new Mock<IExchangeSettings>();
            _exchangeSettingsMock.Setup(x => x.ExcludedCurrencies).Returns(new List<string> { "XYZ" });
            _validator = new ConvertCurrencyQueryValidator(_exchangeSettingsMock.Object);
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Less_Than_Or_Equal_To_Zero()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 0,
                    BaseCurrency = "USD",
                    TargetCurrency = "EUR"
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ConversionRequest.Amount);
        }

        [Fact]
        public void Should_Have_Error_When_BaseCurrency_Is_Empty()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 100,
                    BaseCurrency = "",
                    TargetCurrency = "EUR"
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ConversionRequest.BaseCurrency);
        }

        [Fact]
        public void Should_Have_Error_When_TargetCurrency_Is_Empty()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 100,
                    BaseCurrency = "USD",
                    TargetCurrency = ""
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ConversionRequest.TargetCurrency);
        }

        [Fact]
        public void Should_Have_Error_When_BaseCurrency_Is_Excluded()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 100,
                    BaseCurrency = "XYZ",
                    TargetCurrency = "EUR"
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ConversionRequest.BaseCurrency)
                  .WithErrorMessage("Base currency excluded from the list.");
        }

        [Fact]
        public void Should_Have_Error_When_TargetCurrency_Is_Excluded()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 100,
                    BaseCurrency = "USD",
                    TargetCurrency = "XYZ"
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.ConversionRequest.TargetCurrency)
                  .WithErrorMessage("Target currency excluded from the list.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Request()
        {
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    Amount = 100,
                    BaseCurrency = "USD",
                    TargetCurrency = "EUR"
                }
            };

            var result = _validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
