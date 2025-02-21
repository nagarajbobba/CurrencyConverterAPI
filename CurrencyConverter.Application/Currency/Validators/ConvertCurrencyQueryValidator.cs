using CurrencyConverter.Application.Currency.Interfaces;
using CurrencyConverter.Application.Currency.Queries;
using FluentValidation;

namespace CurrencyConverter.Application.Currency.Validators
{
    public class ConvertCurrencyQueryValidator : AbstractValidator<ConvertCurrencyQuery>
    {
        private readonly IExchangeSettings _exchangeSettings;
       
        public ConvertCurrencyQueryValidator(IExchangeSettings exchangeSettings)
        {
            _exchangeSettings = exchangeSettings;
            RuleFor(x => x.ConversionRequest.Amount).GreaterThan(0);
            RuleFor(x => x.ConversionRequest.TargetCurrency).NotEmpty();
            RuleFor(x => x.ConversionRequest.BaseCurrency).NotEmpty();
            RuleFor(x => x.ConversionRequest.BaseCurrency)
               .Must(IsValidCurrency)
               .WithMessage("Base currency excluded from the list.");

            RuleFor(x => x.ConversionRequest.TargetCurrency)
                .Must(IsValidCurrency)
                .WithMessage("Target currency excluded from the list.");
        }
        private bool IsValidCurrency(string currency)
        {
            return !_exchangeSettings.ExcludedCurrencies.Contains(currency);
        }
    }
}
