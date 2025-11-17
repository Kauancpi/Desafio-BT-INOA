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

namespace stock_alert
{
    class Program
    {
        //Funcao pra mandar um email
        public static void SendEmail()
        {
            // Configure API key authorization: api-key
            Configuration.Default.ApiKey.Add("api-key", "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");

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
        static async System.Threading.Tasks.Task Main()
        {
            //Parte do código que recebe o preço da API
            string token = "eoBmtRMk6mhUioq9hqrGtM";
            string ticker = "PETR4";
            string url = $"https://brapi.dev/api/quote/{ticker}?token={token}";
            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            int precopos1 = response.IndexOf("\"regularMarketPrice\":");
            int precopos2 = response.IndexOf(",\"regularMarketDayHigh\":");
            
            string preco = response.Substring(precopos1+21,precopos2-precopos1-21);


            float currentprice = float.Parse(preco,System.Globalization.CultureInfo.InvariantCulture);

            SendEmail();

            
            }
            
    }
}