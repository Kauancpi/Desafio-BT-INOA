using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;


namespace stock_alert
{
    class Program
    {

        private static IConfiguration? _configuration;

        private static void LoadConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        //Funcao pra mandar um email
        public static async Task SendEmail(bool sell, string ticker, float precoLimite)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration not loaded");
            }

            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var useSsl = bool.Parse(_configuration["EmailSettings:UseSsl"]); 
            var destinationemail = _configuration["DestinationEmail"];
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", destinationemail));
            message.Subject = "Aviso a respeito do preço da ação";

            if(sell == true){
                message.Body = new TextPart("plain")
                {
                Text = $"O preço da ação '{ticker}' passou de R${precoLimite}, recomendo vender"
                };
            }
            else
            {
                message.Body = new TextPart("plain")
                {
                Text = $"O preço da ação '{ticker}' está abaixo de  R${precoLimite}, recomendo comprar"
                };
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.Auto);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public static async Task<float> GetPrice(string ticker)
        {
            if (_configuration == null){
                throw new InvalidOperationException("Configuration not loaded");
            }

            string token = _configuration["BrapiSettings:Token"]!; 
            string url = $"https://brapi.dev/api/quote/{ticker}?token={token}";
            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);
            int precopos1 = response.IndexOf("\"regularMarketPrice\":");
            int precopos2 = response.IndexOf(",\"regularMarketDayHigh\":");
            string preco = response.Substring(precopos1+21,precopos2-precopos1-21);
            float currentprice = float.Parse(preco,System.Globalization.CultureInfo.InvariantCulture);
            return currentprice;
        }

        static async Task Main(string[] args)
        {
            LoadConfiguration();
            
            if (args.Length < 3)
            {
                Console.WriteLine("Uso: stock-quote-alert.exe <TICKER> <PRECO_VENDA> <PRECO_COMPRA>");
                Console.WriteLine("Exemplo: stock-quote-alert.exe PETR4 22.67 22.59");
                return;
            }

            string ticker = args[0].ToUpper();
            
            if (!float.TryParse(args[1], System.Globalization.CultureInfo.InvariantCulture, out float precoVenda))
            {
                Console.WriteLine($"Erro: '{args[1]}' não é um preço válido");
                return;
            }

            if (!float.TryParse(args[2], System.Globalization.CultureInfo.InvariantCulture, out float precoCompra))
            {
                Console.WriteLine($"Erro: '{args[2]}' não é um preço válido");
                return;
            }

            if (precoCompra > precoVenda)
            {
                Console.WriteLine("Erro, preço de venda tem que ser maior ou igual ao de compra, por favor coloque o maior número primeiro.");
                return;
            }

            Console.WriteLine($"Monitorando {ticker}");
            Console.WriteLine($"Preço de venda: R$ {precoVenda:F2}");
            Console.WriteLine($"Preço de compra: R$ {precoCompra:F2}");

            
            while(true){
            
            
            try
            {
                float precoAtual = await GetPrice(ticker);
                Console.WriteLine($"Preço atual: R$ {precoAtual:F2}");

                // Verificar alertas
                if (precoAtual >= precoVenda)
                {
                    await SendEmail(true, ticker, precoVenda);
                }
                else if (precoAtual <= precoCompra)
                {
                    await SendEmail(false, ticker, precoCompra);
                }
                else
                {
                    Console.WriteLine("Preço dentro do intervalo. Nenhum alerta enviado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            
            Console.WriteLine("Aguardando 5 minutos...\n");
            await Task.Delay(TimeSpan.FromMinutes(5));
            
            }

             
        }
            
    }
}