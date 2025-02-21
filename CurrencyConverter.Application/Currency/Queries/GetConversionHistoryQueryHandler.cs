using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class GetConversionHistoryQueryHandler : IRequestHandler<GetConversionHistoryQuery, CurrencyRatesHistoryResponse>
    {       
        private readonly IValidator<GetConversionHistoryQuery> _validator;
        private readonly ICurrencyProviderFactory _currencyProviderFactory;
        private readonly IExchangeSettings _exchangeSettings;
        public GetConversionHistoryQueryHandler(IValidator<GetConversionHistoryQuery> validator,
            ICurrencyProviderFactory currencyProviderFactory,
            IExchangeSettings exchangeSettings)
        {           
            _validator = validator;
            _currencyProviderFactory = currencyProviderFactory;
            _exchangeSettings = exchangeSettings;
        }
        public async Task<CurrencyRatesHistoryResponse> Handle(GetConversionHistoryQuery request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get the provider from the factory
            ICurrencyProviderReadRepository provider = _currencyProviderFactory.GetProvider(_exchangeSettings.ActiveProvider);
            return await provider.GetHistoricalRatesAsync(request.HistoryRequest, cancellationToken);
        }
    }
}
