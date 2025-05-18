using System;

namespace AkademiqRapidApiCase.Models
{
    public class CurrencyResponse
    {
        public string Status { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Amount { get; set; }
        public double Result { get; set; }
        public DateTime Date { get; set; }
    }
} 