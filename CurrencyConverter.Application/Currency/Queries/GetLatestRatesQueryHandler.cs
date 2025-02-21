using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Core.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class GetLatestRatesQueryHandler : IRequestHandler<GetLatestRatesQuery, CurrencyRates>
    {       
        private readonly IValidator<GetLatestRatesQuery> _validator;
        private readonly ICurrencyProviderFactory _currencyProviderFactory;
        private readonly IExchangeSettings _exchangeSettings;

        public GetLatestRatesQueryHandler(IValidator<GetLatestRatesQuery> validator,
            ICurrencyProviderFactory currencyProviderFactory,
            IExchangeSettings exchangeSettings)
        {           
            _validator = validator;
            _currencyProviderFactory = currencyProviderFactory;
            _exchangeSettings = exchangeSettings;
        }
        public async Task<CurrencyRates> Handle(GetLatestRatesQuery request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get the provider from the factory
            ICurrencyProviderReadRepository provider = _currencyProviderFactory.GetProvider(_exchangeSettings.ActiveProvider);
            return await provider.GetLatestRatesAsync(request.BaseCurrency, cancellationToken);
        }
    }
}
