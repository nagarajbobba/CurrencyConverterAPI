using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Application.Currency.Queries;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CurrencyConverter.Application.Tests.Currency
{
    public class ConvertCurrencyQueryHandlerTests
    {
        private readonly Mock<IValidator<ConvertCurrencyQuery>> _validatorMock;
        private readonly Mock<ICurrencyProviderFactory> _currencyProviderFactoryMock;
        private readonly Mock<IExchangeSettings> _exchangeSettingsMock;
        private readonly ConvertCurrencyQueryHandler _handler;

        public ConvertCurrencyQueryHandlerTests()
        {
            _validatorMock = new Mock<IValidator<ConvertCurrencyQuery>>();
            _currencyProviderFactoryMock = new Mock<ICurrencyProviderFactory>();
            _exchangeSettingsMock = new Mock<IExchangeSettings>();
            _handler = new ConvertCurrencyQueryHandler(_validatorMock.Object, _currencyProviderFactoryMock.Object, _exchangeSettingsMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnConversionResponse_WhenRequestIsValid()
        {
            // Arrange
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    BaseCurrency = "USD",
                    TargetCurrency = "EUR",
                    Amount = 100
                }
            };
            var validationResult = new ValidationResult();
            var expectedResponse = new CurrencyConversionResponse
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100,
                ConvertedAmount = 85
            };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("TestProvider");

            var providerMock = new Mock<ICurrencyProviderReadRepository>();
            providerMock.Setup(p => p.ConvertAsync(query.ConversionRequest, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            _currencyProviderFactoryMock.Setup(f => f.GetProvider("TestProvider")).Returns(providerMock.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            // Arrange
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    BaseCurrency = "USD",
                    TargetCurrency = "EUR",
                    Amount = 100
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("BaseCurrency", "Invalid currency") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenProviderNotFound()
        {
            // Arrange
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    BaseCurrency = "USD",
                    TargetCurrency = "EUR",
                    Amount = 100
                }
            };
            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("NonExistentProvider");

            _currencyProviderFactoryMock.Setup(f => f.GetProvider("NonExistentProvider")).Throws(new Exception("Provider not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenBaseCurrencyIsExcluded()
        {
            // Arrange
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    BaseCurrency = "EXC",
                    TargetCurrency = "EUR",
                    Amount = 100
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("BaseCurrency", "Base currency excluded from the list.") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ExcludedCurrencies).Returns(new List<string> { "EXC" });

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenTargetCurrencyIsExcluded()
        {
            // Arrange
            var query = new ConvertCurrencyQuery
            {
                ConversionRequest = new CurrencyConversionRequest
                {
                    BaseCurrency = "USD",
                    TargetCurrency = "EXC",
                    Amount = 100
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("TargetCurrency", "Target currency excluded from the list.") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ExcludedCurrencies).Returns(new List<string> { "EXC" });

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
