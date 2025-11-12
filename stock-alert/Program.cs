using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace stock_alert
{
    public class Quote
    {
        public string Symbol { get; set; }
        public string ShortName { get; set; }
        public decimal RegularMarketPrice { get; set; }
        public decimal RegularMarketChangePercent { get; set; }
        public string Currency { get; set; }
    }
    public class QuoteResponse
    {
        public Quote[] Results { get; set; }
    }
    public class BrapiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private const string BaseUrl = "https://brapi.dev/api";
        public BrapiClient(string token)
        {
            _token = token;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }
        public async Task<Quote> GetQuoteAsync(string ticker)
        {
            var url = $"{BaseUrl}/quote/{ticker}?token={_token}";
            var response = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<QuoteResponse>(response);
            return data?.Results?[0];
        }
        static async Task Main()
        {
            var client = new BrapiClient("SEU_TOKEN");
            var quote = await client.GetQuoteAsync("PETR4");

            if (quote != null)
            {
                Console.WriteLine($"{quote.Symbol}: R$ {quote.RegularMarketPrice:F2}");
            }
        }
    }
}