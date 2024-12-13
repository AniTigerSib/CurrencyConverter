using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateResponse> GetExchangeRatesAsync(DateTime date);
    }

    public class CbrExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient = new();
        private const string BaseUrl = "http://www.cbr-xml-daily.ru";

        public async Task<ExchangeRateResponse> GetExchangeRatesAsync(DateTime date)
        {
            string[] possibleUrls = {
                date.Date == DateTime.Today.Date
                    ? $"https://www.cbr-xml-daily.ru/daily_json.js"
                    : $"https://www.cbr-xml-daily.ru/archive/{date:yyyy}/{date:MM}/{date:dd}/daily_json.js",

                date.Date == DateTime.Today.Date
                    ? $"https://cbr-xml-daily.ru/daily_json.js"
                    : $"https://cbr-xml-daily.ru/archive/{date:yyyy}/{date:MM}/{date:dd}/daily_json.js"
            };

            foreach (var url in possibleUrls)
            {
                try
                {
                    // Добавляем User-Agent для предотвращения блокировок
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CurrencyConverter/1.0");

                    var response = await _httpClient.GetStringAsync(url);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var exchangeRates = JsonSerializer.Deserialize<ExchangeRateResponse>(response, options);

                    // Добавляем рубль в список валют
                    if (exchangeRates is { Valute: not null })
                    {
                        exchangeRates.Valute["RUB"] = new Currency
                        {
                            CharCode = "RUB",
                            Name = "Российский рубль",
                            Value = 1,
                            Nominal = 1
                        };
                    }

                    return exchangeRates;
                }
                catch (HttpRequestException ex)
                {
                    // Логирование ошибки (можно заменить на вашу систему логирования)
                    System.Diagnostics.Debug.WriteLine($"Ошибка при запросе {url}: {ex.Message}");
                    throw new Exception();
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка парсинга JSON {url}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Неожиданная ошибка {url}: {ex.Message}");
                }
            }

            throw new Exception("Не удалось получить курсы валют. Проверьте интернет-соединение.");
        }
    }
}
