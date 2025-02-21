namespace CurrencyConverter.Application.Currency.Interfaces
{
    public interface IExchangeSettings
    {
        public List<string> ExcludedCurrencies { get; set; }
        public string FrankFurterApiProvider { get; set; }
        public string AnotherExchangeRatesApiProvider { get; set; }
        public string ActiveProvider { get; set; }
    }
}
