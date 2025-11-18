# Desafio-BT-INOA
Programa para o processo seletivo da INOA


O programa recebe um arquivo do tipo json da seguinte forma:

{
  "EmailSettings": {
    "SmtpServer": "smtp.server.com",
    "SmtpPort": 465,
    "SenderName": "Stock-Alert",
    "SenderEmail": "senderemail",
    "Username": "username",
    "Password": "password",
    "UseSsl": true,
    "DestinationEmail": "destination"
  },
  "BrapiSettings": {
    "Token": "token_brapi"
  }
}

A API da brapi que estou usando atualiza o preço a cada 30 minutos, entao o codigo tem um delay de 15 min entre as requisiçoes, da pra mudar esse timing dependendo.


O source code esta em stock-alert/Program.cs
