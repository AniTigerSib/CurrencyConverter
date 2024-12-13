using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class Currency
    {
        public string CharCode { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int Nominal { get; set; }
    }

    public class ExchangeRateResponse
    {
        public DateTime Date { get; set; }
        public Dictionary<string, Currency> Valute { get; set; }
    }
}
