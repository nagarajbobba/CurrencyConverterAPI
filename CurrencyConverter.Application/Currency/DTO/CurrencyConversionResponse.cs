namespace CurrencyConverter.Application.Currency.DTO
{
    public class CurrencyConversionResponse : CurrencyConversionRequest
    {
        public decimal ConvertedAmount { get; set; }
    }
}
