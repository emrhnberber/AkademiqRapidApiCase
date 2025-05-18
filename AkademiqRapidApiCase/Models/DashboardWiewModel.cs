using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace AkademiqRapidApiCase.Models

{
    public class DashboardWiewModel
    {
        public CurrencyResponse EurTryRate { get; set; }
        public CurrencyResponse UsdTryRate { get; set; }
        public FuelPriceResponse FuelPrices { get; set; }
        public WeatherResponse Weather { get; set; }
        public List<CryptoPrice> CryptoPrices { get; set; }
        public RecipeResponse Recipe { get; set; }
        public Meal RandomMeal { get; set; }
        public NewsResponse News { get; set; }
        public List<NewsItem> LastNews { get; set; }
        public string FuelRawJson { get; set; }
        public string WeatherRawJson { get; set; }
    }
}
