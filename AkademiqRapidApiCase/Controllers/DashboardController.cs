using AkademiqRapidApiCase.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AkademiqRapidApiCase.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _client;
        private const string API_KEY = "35f55a86d2msh4846f75c6ac6fe0p16367ejsnd4a2ee6eb375";
        private const string CURRENCY_API_HOST = "currency-conversion-and-exchange-rates.p.rapidapi.com";
        private const string FUEL_API_HOST = "akaryakit-fiyatlari.p.rapidapi.com";

        public DashboardController()
        {
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardWiewModel();

            try
            {
                // Döviz kurları için API çağrıları
                var eurRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{CURRENCY_API_HOST}/convert?from=EUR&to=TRY&amount=1"),
                    Headers =
                    {
                        { "x-rapidapi-key", API_KEY },
                        { "x-rapidapi-host", CURRENCY_API_HOST },
                    },
                };

                var usdRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{CURRENCY_API_HOST}/convert?from=USD&to=TRY&amount=1"),
                    Headers =
                    {
                        { "x-rapidapi-key", API_KEY },
                        { "x-rapidapi-host", CURRENCY_API_HOST },
                    },
                };

                // Akaryakıt fiyatları için API çağrısı
                var fuelRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://{FUEL_API_HOST}/fuel/istanbul"),
                    Headers =
                    {
                        { "x-rapidapi-key", "1043c4e4cemsh21ae76022d8a618p1d68c8jsn867a49c8782a" },
                        { "x-rapidapi-host", FUEL_API_HOST },
                    },
                };
                
                // Tüm API çağrılarını paralel olarak yap
                var eurResponse = await _client.SendAsync(eurRequest);
                var usdResponse = await _client.SendAsync(usdRequest);
                var fuelResponse = await _client.SendAsync(fuelRequest);

                eurResponse.EnsureSuccessStatusCode();
                usdResponse.EnsureSuccessStatusCode();
                fuelResponse.EnsureSuccessStatusCode();

                var eurBody = await eurResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("EUR API: " + eurBody);
                if (eurBody.Contains("rate limit") || eurBody.Contains("quota") || eurResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Döviz API istek hakkınız doldu.");

                var usdBody = await usdResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("USD API: " + usdBody);
                if (usdBody.Contains("rate limit") || usdBody.Contains("quota") || usdResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Döviz API istek hakkınız doldu.");

                var fuelBody = await fuelResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("FUEL API: " + fuelBody);
                if (fuelBody.Contains("rate limit") || fuelBody.Contains("quota") || fuelResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Akaryakıt API istek hakkınız doldu.");

                viewModel.EurTryRate = JsonConvert.DeserializeObject<CurrencyResponse>(eurBody);
                viewModel.UsdTryRate = JsonConvert.DeserializeObject<CurrencyResponse>(usdBody);
                viewModel.FuelPrices = JsonConvert.DeserializeObject<FuelPriceResponse>(fuelBody);
                viewModel.FuelRawJson = fuelBody;

                // Hava durumu API çağrısı
                var weatherRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://weather-api167.p.rapidapi.com/api/weather/forecast?place=Istanbul%2CTR&cnt=3&units=metric&type=three_hour&mode=json&lang=tr"),
                    Headers =
                    {
                        { "x-rapidapi-key", "1043c4e4cemsh21ae76022d8a618p1d68c8jsn867a49c8782a" },
                        { "x-rapidapi-host", "weather-api167.p.rapidapi.com" },
                        { "Accept", "application/json" },
                    },
                };
                var weatherResponse = await _client.SendAsync(weatherRequest);
                weatherResponse.EnsureSuccessStatusCode();
                var weatherBody = await weatherResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("WEATHER API: " + weatherBody);
                if (weatherBody.Contains("rate limit") || weatherBody.Contains("quota") || weatherResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Hava durumu API istek hakkınız doldu.");
                viewModel.WeatherRawJson = weatherBody;
                viewModel.Weather = JsonConvert.DeserializeObject<WeatherResponse>(weatherBody);

                // Binance API çağrısı
                var cryptoRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://binance43.p.rapidapi.com/ticker/24hr"),
                    Headers =
                    {
                        { "x-rapidapi-key", API_KEY },
                        { "x-rapidapi-host", "binance43.p.rapidapi.com" },
                    },
                };
                var cryptoResponse = await _client.SendAsync(cryptoRequest);
                cryptoResponse.EnsureSuccessStatusCode();
                var cryptoBody = await cryptoResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("CRYPTO API: " + cryptoBody);
                if (cryptoBody.Contains("rate limit") || cryptoBody.Contains("quota") || cryptoResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Kripto API istek hakkınız doldu.");
                var allCoins = JsonConvert.DeserializeObject<List<CryptoPrice>>(cryptoBody);
                if (allCoins == null || allCoins.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("CRYPTO DESERIALIZE SORUNU!");
                }
                viewModel.CryptoPrices = allCoins?.Where(c => c.symbol == "ETHUSDT" || c.symbol == "DOGEUSDT" || c.symbol == "SOLUSDT" || c.symbol == "JUPUSDT").ToList();

                // Yemek tarifi API çağrısı
                var recipeRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://themealdb.p.rapidapi.com/search.php?f=a"),
                    Headers =
                    {
                        { "x-rapidapi-key", "1043c4e4cemsh21ae76022d8a618p1d68c8jsn867a49c8782a" },
                        { "x-rapidapi-host", "themealdb.p.rapidapi.com" },
                    },
                };
                var recipeResponse = await _client.SendAsync(recipeRequest);
                recipeResponse.EnsureSuccessStatusCode();
                var recipeBody = await recipeResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("RECIPE API: " + recipeBody);
                viewModel.Recipe = JsonConvert.DeserializeObject<RecipeResponse>(recipeBody);
                if (viewModel.Recipe?.meals != null && viewModel.Recipe.meals.Count > 0)
                {
                    var rnd = new System.Random();
                    viewModel.RandomMeal = viewModel.Recipe.meals[rnd.Next(viewModel.Recipe.meals.Count)];
                }

                // Haber API çağrısı
                var newsRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://real-time-news-data.p.rapidapi.com/topic-news-by-section?topic=TECHNOLOGY&section=CAQiSkNCQVNNUW9JTDIwdk1EZGpNWFlTQldWdUxVZENHZ0pKVENJT0NBUWFDZ29JTDIwdk1ETnliSFFxQ2hJSUwyMHZNRE55YkhRb0FBKi4IACoqCAoiJENCQVNGUW9JTDIwdk1EZGpNWFlTQldWdUxVZENHZ0pKVENnQVABUAE&limit=500&country=US&lang=en"),
                    Headers =
                    {
                        { "x-rapidapi-key", "1043c4e4cemsh21ae76022d8a618p1d68c8jsn867a49c8782a" },
                        { "x-rapidapi-host", "real-time-news-data.p.rapidapi.com" },
                    },
                };
                var newsResponse = await _client.SendAsync(newsRequest);
                newsResponse.EnsureSuccessStatusCode();
                var newsBody = await newsResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("NEWS API: " + newsBody);
                viewModel.News = JsonConvert.DeserializeObject<NewsResponse>(newsBody);
                if (viewModel.News?.data != null && viewModel.News.data.Count > 0)
                {
                    viewModel.LastNews = viewModel.News.data.Take(5).ToList();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Veriler alınırken bir hata oluştu: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("EXCEPTION: " + ex.ToString());
            }
            
            return View(viewModel);
        }
    }
}
