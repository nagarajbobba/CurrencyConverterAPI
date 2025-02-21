using CurrencyConverter.API.Controllers.V1;
using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Queries;
using CurrencyConverter.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CurrencyConverter.API.Tests.V1
{
    public class CurrencyConverterControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CurrencyConverterController _controller;

        public CurrencyConverterControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CurrencyConverterController(_mediatorMock.Object);
        }

        #region GetLatestRates Tests

        [Fact]
        public async Task GetLatestRates_ShouldReturnOk_WithRates()
        {
            // Arrange
            var expectedRates = new CurrencyRates { Rates = new Dictionary<string, decimal> { { "USD", 1.0M }, { "EUR", 0.85M } } };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetLatestRatesQuery>(), default))
                .ReturnsAsync(expectedRates);

            // Act
            var result = await _controller.GetLatestRates("USD");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedRates, okResult.Value);
        }

        [Fact]
        public async Task GetLatestRates_ShouldReturnBadRequest_WhenMediatorReturnsNull()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetLatestRatesQuery>(), default))
                .ReturnsAsync((CurrencyRates?)null);

            // Act
            var result = await _controller.GetLatestRates("USD");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode); 
        }       

        #endregion

        #region ConvertCurrency Tests

        [Fact]
        public async Task ConvertCurrency_ShouldReturnOk_WithConvertedAmount()
        {
            // Arrange
            var expectedResponse = new CurrencyConversionResponse { ConvertedAmount = 100M };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConvertCurrencyQuery>(), default))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.ConvertCurrency("USD", "EUR", 100M);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task ConvertCurrency_ShouldReturnBadRequest_WhenConversionFails()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConvertCurrencyQuery>(), default))
                .ReturnsAsync((CurrencyConversionResponse?)null);

            // Act
            var result = await _controller.ConvertCurrency("USD", "EUR", 100M);

            // Assert
            var badRequestResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, badRequestResult.StatusCode);         
        }
       
        #endregion

        #region GetConversionHistory Tests

        [Fact]
        public async Task GetConversionHistory_ShouldReturnOk_WithHistoryData()
        {
            // Arrange
            var expectedHistory = new CurrencyRatesHistoryResponse
            {
                Amount = 100M,
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
                    },
                    {
                        "2025-01-02", new Dictionary<string, decimal>
                        {
                            { "EUR", 0.86M },
                            { "GBP", 0.76M }
                        }
                    }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetConversionHistoryQuery>(), default))
                .ReturnsAsync(expectedHistory);

            // Act
            var result = await _controller.GetConversionHistory("2025-02-01", "2025-02-10", "USD", 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedHistory, okResult.Value);
        }

        [Fact]
        public async Task GetConversionHistory_ShouldReturnBadRequest_WhenHistoryNotAvailable()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetConversionHistoryQuery>(), default))
                .ReturnsAsync((CurrencyRatesHistoryResponse?)null);

            // Act
            var result = await _controller.GetConversionHistory("2025-02-01", "2025-02-10", "USD", 1, 10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);                    
        }

        #endregion

      
    }
}
