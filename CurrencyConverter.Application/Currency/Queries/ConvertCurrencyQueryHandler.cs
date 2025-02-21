using CurrencyConverter.Application.Currency.DTO;
using CurrencyConverter.Application.Currency.Interfaces;
using MediatR;
using CurrencyConverter.Application.Currency.Validators;
using FluentValidation.Results;
using FluentValidation;

namespace CurrencyConverter.Application.Currency.Queries
{
    public class ConvertCurrencyQueryHandler : IRequestHandler<ConvertCurrencyQuery, CurrencyConversionResponse>
    {        
        private readonly IValidator<ConvertCurrencyQuery> _validator;
        private readonly ICurrencyProviderFactory _currencyProviderFactory;
        private readonly IExchangeSettings _exchangeSettings;

        public ConvertCurrencyQueryHandler(IValidator<ConvertCurrencyQuery> validator,
              ICurrencyProviderFactory currencyProviderFactory,
            IExchangeSettings exchangeSettings)
        {
          
            _validator = validator;
            _currencyProviderFactory = currencyProviderFactory;
            _exchangeSettings = exchangeSettings;
        }
        public async Task<CurrencyConversionResponse> Handle(ConvertCurrencyQuery request, CancellationToken cancellationToken)
        {

            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Get the provider from the factory
            ICurrencyProviderReadRepository provider = _currencyProviderFactory.GetProvider(_exchangeSettings.ActiveProvider);

            return await provider.ConvertAsync(request.ConversionRequest, cancellationToken);
        }
    }
}
