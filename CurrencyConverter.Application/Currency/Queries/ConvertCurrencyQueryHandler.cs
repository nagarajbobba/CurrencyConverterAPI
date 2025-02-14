using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class ConvertCurrencyQueryHandler : IRequestHandler<ConvertCurrencyQuery, CurrencyConversionResponse>
    {
        private readonly ICurrencyConverterReadRepository _currencyConverterReadRepository;

        public ConvertCurrencyQueryHandler(ICurrencyConverterReadRepository currencyConverterReadRepository)
        {
            _currencyConverterReadRepository = currencyConverterReadRepository;
        }
        public async Task<CurrencyConversionResponse> Handle(ConvertCurrencyQuery request, CancellationToken cancellationToken)
        {
            return await _currencyConverterReadRepository.ConvertAsync(request.ConversionRequest, cancellationToken);
        }
    }
}
