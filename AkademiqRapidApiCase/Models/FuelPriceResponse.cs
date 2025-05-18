using System.Collections.Generic;

namespace AkademiqRapidApiCase.Models
{
    public class FuelPriceResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<FuelDistrict> data { get; set; }
    }

    public class FuelDistrict
    {
        public string district { get; set; }
        public List<FuelPrice> prices { get; set; }
    }

    public class FuelPrice
    {
        public string dagitici_firma { get; set; }
        public string benzin { get; set; }
        public string motorin { get; set; }
        public string lpg { get; set; }
        public string guncellenme_tarihi { get; set; }
    }
} 