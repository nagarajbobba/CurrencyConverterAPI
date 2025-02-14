using CurrencyConverter.Application.Currency.DTO;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class GetConversionHistoryQuery : IRequest<CurrencyRatesHistoryResponse>
    {
        public CurrencyRatesHistoryRequest HistoryRequest { get; set; }
    }
}
