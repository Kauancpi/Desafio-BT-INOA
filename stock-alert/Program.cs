using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using DotNetEnv;
using System.Runtime.Intrinsics.X86;
using Newtonsoft.Json;


namespace stock_alert
{
    class Program
    {
        //Funcao pra mandar um email
        public static void SendEmail()
        {
            var apiKeyBrevo = Environment.GetEnvironmentVariable("BREVO_API_KEY");
            Configuration.Default.ApiKey.Add("api-key", apiKeyBrevo);
            var apiInstance = new AccountApi();
            try
            {
                // Get your account information, plan and credits details
                GetAccount result = apiInstance.GetAccount();
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AccountApi.GetAccount: " + e.Message );
            }
        }

        public static async Task<float> GetPrice(string ticker)
        {
            string token = Environment.GetEnvironmentVariable("BRAPI_TOKEN");
            string url = $"https://brapi.dev/api/quote/{ticker}?token={token}";
            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            int precopos1 = response.IndexOf("\"regularMarketPrice\":");
            int precopos2 = response.IndexOf(",\"regularMarketDayHigh\":");
            string preco = response.Substring(precopos1+21,precopos2-precopos1-21);
            float currentprice = float.Parse(preco,System.Globalization.CultureInfo.InvariantCulture);
            return currentprice;
        }

        static async System.Threading.Tasks.Task Main()
        {
            DotNetEnv.Env.Load();
            SendEmail(); 
        }
            
    }
}