using CurrencyConverter.Application.Currency.DTO;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class ConvertCurrencyQuery : IRequest<CurrencyConversionResponse>
    {
        public CurrencyConversionRequest ConversionRequest { get; set; }
        
    }
}
