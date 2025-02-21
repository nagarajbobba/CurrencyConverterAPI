using Polly;
using Polly.CircuitBreaker;

namespace CurrencyConverter.Infrastructure.Utility
{
    public class CircuitBreakerPolicyProvider
    {
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>() // Handle HttpRequest exceptions
            .OrResult(r => !r.IsSuccessStatusCode) // Handle unsuccessful HTTP responses
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5, // Break after 50% of requests fail
                samplingDuration: TimeSpan.FromSeconds(10), // The failure threshold is measured over 10 seconds
                minimumThroughput: 8, // Breaker will break only if at least 8 requests were made
                durationOfBreak: TimeSpan.FromSeconds(30)); // Duration of the break
        }
    }
}
