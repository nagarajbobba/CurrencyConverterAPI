using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Core.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CurrencyConverter.Application.Tests.Currency
{
    public class GetLatestRatesQueryHandlerTests
    {
        private readonly Mock<IValidator<GetLatestRatesQuery>> _validatorMock;
        private readonly Mock<ICurrencyProviderFactory> _currencyProviderFactoryMock;
        private readonly Mock<IExchangeSettings> _exchangeSettingsMock;
        private readonly GetLatestRatesQueryHandler _handler;

        public GetLatestRatesQueryHandlerTests()
        {
            _validatorMock = new Mock<IValidator<GetLatestRatesQuery>>();
            _currencyProviderFactoryMock = new Mock<ICurrencyProviderFactory>();
            _exchangeSettingsMock = new Mock<IExchangeSettings>();
            _handler = new GetLatestRatesQueryHandler(_validatorMock.Object, _currencyProviderFactoryMock.Object, _exchangeSettingsMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnRates_WhenRequestIsValid()
        {
            // Arrange
            var query = new GetLatestRatesQuery 
            { 
               BaseCurrency = "USD" 
            };
            var validationResult = new ValidationResult();
            var expectedRates = new CurrencyRates { Rates = new Dictionary<string, decimal> { { "EUR", 0.85M } } };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("TestProvider");

            var providerMock = new Mock<ICurrencyProviderReadRepository>();
            providerMock.Setup(p => p.GetLatestRatesAsync("USD", It.IsAny<CancellationToken>())).ReturnsAsync(expectedRates);

            _currencyProviderFactoryMock.Setup(f => f.GetProvider("TestProvider")).Returns(providerMock.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedRates, result);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            // Arrange
            var query = new GetLatestRatesQuery
            {
                BaseCurrency = "USD"
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
            var query = new GetLatestRatesQuery
            {
                BaseCurrency = "USD"
            };
            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("NonExistentProvider");

            _currencyProviderFactoryMock.Setup(f => f.GetProvider("NonExistentProvider")).Throws(new Exception("Provider not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
        
        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenBaseCurrencyIsNullOrEmpty()
        {
            // Arrange
            var query = new GetLatestRatesQuery
            {
                BaseCurrency = string.Empty
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("BaseCurrency", "Base currency is required") });
            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
