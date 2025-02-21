namespace CurrencyConverter.Application.Currency.Interfaces
{
    public interface ICurrencyProviderFactory
    {
        ICurrencyProviderReadRepository GetProvider(string providerType);
    }
}
