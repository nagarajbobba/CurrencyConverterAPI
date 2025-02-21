using CurrencyConverter.API.Configuration.Interfaces;

namespace CurrencyConverter.API.Configuration
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        // Private field to hold the correlation ID
        private string _correlationId = Guid.NewGuid().ToString();

        // Method to get the current correlation ID
        public string Get() => _correlationId;

        // Method to set a new correlation ID
        public void Set(string CorrelationId) => _correlationId = CorrelationId;
    }
}
