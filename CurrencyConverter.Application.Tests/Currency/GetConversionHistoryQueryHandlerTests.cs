using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Application.Currency.Queries;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.Tests.Currency
{
    public class GetConversionHistoryQueryHandlerTests
    {
        private readonly Mock<IValidator<GetConversionHistoryQuery>> _validatorMock;
        private readonly Mock<ICurrencyProviderFactory> _currencyProviderFactoryMock;
        private readonly Mock<IExchangeSettings> _exchangeSettingsMock;
        private readonly GetConversionHistoryQueryHandler _handler;

        public GetConversionHistoryQueryHandlerTests()
        {
            _validatorMock = new Mock<IValidator<GetConversionHistoryQuery>>();
            _currencyProviderFactoryMock = new Mock<ICurrencyProviderFactory>();
            _exchangeSettingsMock = new Mock<IExchangeSettings>();
            _handler = new GetConversionHistoryQueryHandler(_validatorMock.Object, _currencyProviderFactoryMock.Object, _exchangeSettingsMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnHistoryResponse_WhenRequestIsValid()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult();
            var expectedResponse = new CurrencyRatesHistoryResponse
            {
                Base = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                    {
                        {
                            "2025-01-01", new Dictionary<string, decimal>
                            {
                                { "EUR", 0.85M },
                                { "GBP", 0.75M }
                            }
                        }
                    }
            };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("TestProvider");

            var providerMock = new Mock<ICurrencyProviderReadRepository>();
            providerMock.Setup(p => p.GetHistoricalRatesAsync(query.HistoryRequest, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

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
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 10
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
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 10
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
        public async Task Handle_ShouldThrowValidationException_WhenHistoryRequestIsNull()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = null
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("HistoryRequest", "History request cannot be null") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenBaseCurrencyIsEmpty()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("BaseCurrency", "Base currency cannot be empty") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenStartDateIsEmpty()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("StartDate", "Start date cannot be empty") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenEndDateIsEmpty()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "",
                    Page = 1,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("EndDate", "End date cannot be empty") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenStartDateIsAfterEndDate()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-10",
                    EndDate = "2025-01-01",
                    Page = 1,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("StartDate", "Start date cannot be after end date") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenPageNumberIsInvalid()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 0,
                    PageSize = 10
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Page", "Page number must be greater than zero") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenPageSizeIsInvalid()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 1,
                    PageSize = 0
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("PageSize", "Page size must be greater than zero") });

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnHistoryResponse_WithPagination()
        {
            // Arrange
            var query = new GetConversionHistoryQuery
            {
                HistoryRequest = new CurrencyRatesHistoryRequest
                {
                    BaseCurrency = "USD",
                    StartDate = "2025-01-01",
                    EndDate = "2025-01-10",
                    Page = 2,
                    PageSize = 5
                }
            };
            var validationResult = new ValidationResult();
            var expectedResponse = new CurrencyRatesHistoryResponse
            {
                Base = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                    {
                        {
                            "2025-01-06", new Dictionary<string, decimal>
                            {
                                { "EUR", 0.85M },
                                { "GBP", 0.75M }
                            }
                        }
                    }
            };

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _exchangeSettingsMock.SetupGet(e => e.ActiveProvider).Returns("TestProvider");

            var providerMock = new Mock<ICurrencyProviderReadRepository>();
            providerMock.Setup(p => p.GetHistoricalRatesAsync(query.HistoryRequest, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

            _currencyProviderFactoryMock.Setup(f => f.GetProvider("TestProvider")).Returns(providerMock.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse, result);
        }
    }
}
