namespace CurrencyConverter.API.Configuration.Interfaces
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string CorrelationId);
    }
}
