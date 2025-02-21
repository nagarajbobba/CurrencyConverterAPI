using CurrencyConverter.Core.Entities;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public record GetLatestRatesQuery : IRequest<CurrencyRates>
    {
        public string BaseCurrency { get; init; }
    }
}
