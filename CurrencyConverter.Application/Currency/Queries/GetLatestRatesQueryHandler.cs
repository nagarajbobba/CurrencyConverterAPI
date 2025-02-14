using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Core.Entities;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class GetLatestRatesQueryHandler : IRequestHandler<GetLatestRatesQuery, CurrencyRates>
    {
        private readonly ICurrencyConverterReadRepository _currencyConverterReadRepository;

        public GetLatestRatesQueryHandler(ICurrencyConverterReadRepository currencyConverterReadRepository)
        {
            _currencyConverterReadRepository = currencyConverterReadRepository;
        }
        public async Task<CurrencyRates> Handle(GetLatestRatesQuery request, CancellationToken cancellationToken)
        {
          return await _currencyConverterReadRepository.GetLatestRatesAsync(request.BaseCurrency, cancellationToken);
        }
    }
}
