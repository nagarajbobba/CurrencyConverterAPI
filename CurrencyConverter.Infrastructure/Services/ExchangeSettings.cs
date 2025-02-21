using CurrencyConverter.Application.Currency.Interfaces;

namespace CurrencyConverter.Infrastructure.Services
{
    public class ExchangeSettings : IExchangeSettings
    {
        public List<string> ExcludedCurrencies { get; set; }
        public string FrankFurterApiProvider { get; set; }
        public string AnotherExchangeRatesApiProvider { get; set; }
        public string ActiveProvider { get; set; }
    }
}
