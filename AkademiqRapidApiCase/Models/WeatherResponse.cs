using System.Collections.Generic;

namespace AkademiqRapidApiCase.Models
{
    public class WeatherResponse
    {
        public string cod { get; set; }
        public int message { get; set; }
        public int cnt { get; set; }
        public List<WeatherForecast> list { get; set; }
    }

    public class WeatherForecast
    {
        public long dt { get; set; }
        public string summery { get; set; }
        public MainWeather main { get; set; }
    }

    public class MainWeather
    {
        public double temp { get; set; }
    }
} 