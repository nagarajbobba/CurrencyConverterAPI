using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;

namespace CurrencyConverter.API.IntegrationTests.V1
{
    public class CurrencyConverterIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        public CurrencyConverterIntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [Fact(Skip = "true")]
        public async Task ConvertCurrency_ShouldReturn429TooManyRequests_WhenRateLimitIsExceeded()
        {
            // Arrange: Create a client to simulate HTTP requests
            var client = _factory.CreateClient();

            // Act: Make multiple requests to exceed the rate limit
            for (int i = 0; i < 10; i++)
            {
                var response = await client.GetAsync("/api/CurrencyConverter/convert?baseCurrency=USD&targetCurrency=EUR&amount=100");

                if (i < 5)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Assuming the first few requests pass
                }
                else
                {
                    // Assert: Once the rate limit is exceeded, expect a 429 response
                    Assert.Equal((HttpStatusCode)429, response.StatusCode);
                    break;
                }
            }
        }

    }
}
