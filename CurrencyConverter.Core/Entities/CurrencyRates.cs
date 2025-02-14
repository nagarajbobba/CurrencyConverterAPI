using System.Text.Json.Serialization;

namespace CurrencyConverter.Core.Entities
{
    public class CurrencyRates
    {       
            [JsonPropertyName("amount")]
            public decimal Amount { get; set; }

            [JsonPropertyName("base")]
            public string Base { get; set; }

            [JsonPropertyName("date")]
            public string Date { get; set; }

            [JsonPropertyName("rates")]
            public Dictionary<string, decimal> Rates { get; set; }        
    }
}
