using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using MailKit.Security;

namespace stock_alert
{
    class Program
    {
        //Funcao pra mandar um email
        public static void SendEmail(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("stockalertkauan", "stockalertkauan@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = messageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate("stockalertkauan@gmail.com", "kcci zdov vtju wygu");
                client.Send(message);
                client.Disconnect(true);
            }
        }
        static async Task Main()
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

            

            SendEmail("kauancpi@gmail.com","Teste","teste");
            }
            
    }
}