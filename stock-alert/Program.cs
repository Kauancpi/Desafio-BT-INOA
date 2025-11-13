using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace stock_alert
{
    class Program
    {
        static async Task Main()
        {
            string token = "eoBmtRMk6mhUioq9hqrGtM";
            string ticker = "PETR4";
            string url = $"https://brapi.dev/api/quote/{ticker}?token={token}";
            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            int precopos1 = response.IndexOf("\"regularMarketPrice\":");
            int precopos2 = response.IndexOf(",\"regularMarketDayHigh\":");
            
            string preco = response.Substring(precopos1+21,precopos2-precopos1-21);


            float currentprice = float.Parse(preco,System.Globalization.CultureInfo.InvariantCulture);

        }
    }
}