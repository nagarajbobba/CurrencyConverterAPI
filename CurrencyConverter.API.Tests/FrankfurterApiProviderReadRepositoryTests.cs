using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Catching;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Infrastructure.CurrencyConverter;
using CurrencyConverter.Infrastructure.Utility.Interfaces;
using Moq;
using Polly.CircuitBreaker;
using System.Net;
using System.Text.Json;
using Xunit;

namespace CurrencyConverter.API.Tests
{
    public class FrankfurterApiProviderReadRepositoryTests
    {
        private const string _baseUrl = "https://api.frankfurter.dev/v1/";
        private readonly Mock<IHttpClientExtensions> _httpClientMock;
        private readonly Mock<IExchangeSettings> _exchangeSettingsMock;
        private readonly Mock<ICacheHelper> _cacheHelperMock;
        private readonly FrankfurterApiProviderReadRepository _repository;

        public FrankfurterApiProviderReadRepositoryTests()
        {
            _httpClientMock = new Mock<IHttpClientExtensions>();
            _exchangeSettingsMock = new Mock<IExchangeSettings>();
            _cacheHelperMock = new Mock<ICacheHelper>();
            _repository = new FrankfurterApiProviderReadRepository( _exchangeSettingsMock.Object, _cacheHelperMock.Object, _httpClientMock.Object);
        }

        [Fact]
        public async Task ConvertAsync_ShouldReturnConversionResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100
            };
            var responseContent = JsonSerializer.Serialize(new CurrencyRates
            {
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85M } }
            });
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _repository.ConvertAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal("USD", result.BaseCurrency);
            Assert.Equal("EUR", result.TargetCurrency);
            Assert.Equal(100, result.Amount);
            Assert.Equal(85, result.ConvertedAmount);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldReturnLatestRates_WhenRequestIsValid()
        {
            // Arrange
            var baseCurrency = "USD";
            var responseContent = JsonSerializer.Serialize(new CurrencyRates
            {
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85M } }
            });
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _repository.GetLatestRatesAsync(baseCurrency, CancellationToken.None);

            // Assert
            Assert.Equal(0.85M, result.Rates["EUR"]);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldReturnHistoricalRates_WhenRequestIsValid()
        {
            // Arrange
            var request = new CurrencyRatesHistoryRequest
            {
                BaseCurrency = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Page = 1,
                PageSize = 10
            };
            var responseContent = JsonSerializer.Serialize(new CurrencyRatesHistoryResponse
            {
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                    {
                        { "2025-01-01", new Dictionary<string, decimal> { { "EUR", 0.85M } } }
                    }
            });
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContent)
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _repository.GetHistoricalRatesAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(0.85M, result.Rates["2025-01-01"]["EUR"]);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldReturnCachedRates_WhenCacheHit()
        {
            // Arrange
            var baseCurrency = "USD";
            var cachedRates = new CurrencyRates
            {
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85M } }
            };

            _cacheHelperMock.Setup(c => c.GetAsync<CurrencyRates>(It.IsAny<string>())).ReturnsAsync(new CacheItem<CurrencyRates> { Value = cachedRates });

            // Act
            var result = await _repository.GetLatestRatesAsync(baseCurrency, CancellationToken.None);

            // Assert
            Assert.Equal(0.85M, result.Rates["EUR"]);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldReturnCachedRates_WhenCacheHit()
        {
            // Arrange
            var request = new CurrencyRatesHistoryRequest
            {
                BaseCurrency = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Page = 1,
                PageSize = 10
            };
            var cachedResponse = new CurrencyRatesHistoryResponse
            {
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                    {
                        { "2025-01-01", new Dictionary<string, decimal> { { "EUR", 0.85M } } }
                    }
            };

            _cacheHelperMock.Setup(c => c.GetAsync<CurrencyRatesHistoryResponse>(It.IsAny<string>())).ReturnsAsync(new CacheItem<CurrencyRatesHistoryResponse> { Value = cachedResponse });

            // Act
            var result = await _repository.GetHistoricalRatesAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(0.85M, result.Rates["2025-01-01"]["EUR"]);
        }

        [Fact]
        public async Task ConvertAsync_ShouldThrowHttpRequestException_WhenRequestFails()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _repository.ConvertAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldThrowHttpRequestException_WhenRequestFails()
        {
            // Arrange
            var baseCurrency = "USD";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _repository.GetLatestRatesAsync(baseCurrency, CancellationToken.None));
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldThrowHttpRequestException_WhenRequestFails()
        {
            // Arrange
            var request = new CurrencyRatesHistoryRequest
            {
                BaseCurrency = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Page = 1,
                PageSize = 10
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _repository.GetHistoricalRatesAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task ConvertAsync_ShouldThrowBrokenCircuitException_WhenCircuitBreakerIsOpen()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ThrowsAsync(new BrokenCircuitException());

            // Act & Assert
            await Assert.ThrowsAsync<BrokenCircuitException>(() => _repository.ConvertAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldThrowBrokenCircuitException_WhenCircuitBreakerIsOpen()
        {
            // Arrange
            var baseCurrency = "USD";

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ThrowsAsync(new BrokenCircuitException());

            // Act & Assert
            await Assert.ThrowsAsync<BrokenCircuitException>(() => _repository.GetLatestRatesAsync(baseCurrency, CancellationToken.None));
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldThrowBrokenCircuitException_WhenCircuitBreakerIsOpen()
        {
            // Arrange
            var request = new CurrencyRatesHistoryRequest
            {
                BaseCurrency = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Page = 1,
                PageSize = 10
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ThrowsAsync(new BrokenCircuitException());

            // Act & Assert
            await Assert.ThrowsAsync<BrokenCircuitException>(() => _repository.GetHistoricalRatesAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task ConvertAsync_ShouldThrowInvalidOperationException_WhenDeserializationFails()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid JSON")
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _repository.ConvertAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task GetLatestRatesAsync_ShouldThrowInvalidOperationException_WhenDeserializationFails()
        {
            // Arrange
            var baseCurrency = "USD";
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid JSON")
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _repository.GetLatestRatesAsync(baseCurrency, CancellationToken.None));
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_ShouldThrowInvalidOperationException_WhenDeserializationFails()
        {
            // Arrange
            var request = new CurrencyRatesHistoryRequest
            {
                BaseCurrency = "USD",
                StartDate = "2025-01-01",
                EndDate = "2025-01-10",
                Page = 1,
                PageSize = 10
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid JSON")
            };

            _exchangeSettingsMock.SetupGet(e => e.FrankFurterApiProvider).Returns(_baseUrl);
            _httpClientMock.Setup(c => c.GetWithRetryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>())).ReturnsAsync(responseMessage);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _repository.GetHistoricalRatesAsync(request, CancellationToken.None));
        }
    }
}
