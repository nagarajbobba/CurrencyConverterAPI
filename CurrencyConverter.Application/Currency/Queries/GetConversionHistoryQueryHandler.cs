using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class GetConversionHistoryQueryHandler : IRequestHandler<GetConversionHistoryQuery, CurrencyRatesHistoryResponse>
    {
        private readonly ICurrencyConverterReadRepository _currencyConverterReadRepository;

        public GetConversionHistoryQueryHandler(ICurrencyConverterReadRepository currencyConverterReadRepository)
        {
            _currencyConverterReadRepository = currencyConverterReadRepository;
        }
        public async Task<CurrencyRatesHistoryResponse> Handle(GetConversionHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _currencyConverterReadRepository.GetHistoricalRatesAsync(request.HistoryRequest, cancellationToken);
        }
    }
}
