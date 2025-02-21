
namespace CurrencyConverter.Application.Currency.DTO
{
    public class CurrencyConversionRequest
    {
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Amount { get; set; }
    }
}
