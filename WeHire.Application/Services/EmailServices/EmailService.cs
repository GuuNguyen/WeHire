using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private string _from, _smtpServer, _userName, _password;
        private int _port;

        public EmailService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _from = _configuration["EmailConfig:From"];
            _smtpServer = _configuration["EmailConfig:SmtpServer"];
            _port = Int32.Parse(_configuration["EmailConfig:Port"]);
            _userName = _configuration["EmailConfig:UserName"];
            _password = _configuration["EmailConfig:Password"];
        }

        public async Task SendEmailAsync(string fullName, int userId, string confirmationCode, Message message)
        {
            var emailMessage = CreateEmailMessage(fullName, userId, confirmationCode, message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(string fullName, int userId, string confirmationCode, Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("WeHire", _from));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <!DOCTYPE html>
                <html lang=""en"">

                <head>
                    <meta charset=""UTF-8"">
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>CONFIRM EMAIL</title>
                    <style>
                        @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@500&display=swap');

                        * {{
                            margin: 0;
                            padding: 0;
                            box-sizing: border-box;
                            font-family: 'Poppins', sans-serif;
                        }}
                       .confirm-email {{
                            height: 100vh;
                            width: 50vw;
                            margin: auto;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                        }}
                    </style>
                </head>

                <body>
                    <section class=""confirm-email"">
                        <div class=""container""
                            style=""overflow: hidden; box-shadow: rgba(100, 100, 111, 0.2) 0px 7px 29px 0px;border-radius: 5px; width: 700px; height: 90vh;"">
                            <div class=""confirm-img"">
                                <img style=""height: 300px;width: 700px; object-fit: cover;""
                                    src=""https://blog.trello.com/hubfs/They-Use-Email-You-Use-Trello-final.png"" alt=""Xin chao"">
                            </div>

                            <div class=""confirm-content"" style=""margin-bottom: 2rem; padding: 0 1rem;"">
                                <div class=""content"" style=""margin-bottom: 3rem;"">
                                    <h1 style=""padding-top: 30px; color: #6D5FCF; text-align: center;"">Email Confirmation</h1>
                                    <p style=""padding-top: 30px; font-size: 17px; "">Dear <span
                                            style=""color: #7D8EF0"">{fullName}</span></p>
                                    <p style=""padding-top: 30px; font-size: 15px;"">Thank you for creating a WeHire account!</p>
                                    <p style=""font-size: 15px;"">Please click the button below to complete the registration process.</p>
                                    <p style=""font-size: 15px;"">Failure to confirm your email account within 24 hours will result in
                                        account deletion. If so, you will have to start the membership registration process again and
                                        receive a new confirmation email.
                                    </p>
                                </div>

                                <div class=""confirm-btn"" style=""display: flex; justify-content: center; align-items: center;"">
                                   <a id=""verifyButton"" href=""https://wehireapi.azurewebsites.net/api/Account/Confirm?userId={userId}&confirmationCode={confirmationCode}""
                                   style=""display: inline-block;
                                          background-color: #7D8EF0;
                                          color: white;
                                          text-decoration: none;
                                          padding: 15px 30px;
                                          border-radius: 5px;
                                          font-size: medium;
                                          font-weight: 500;
                                          cursor: pointer;
                                          box-shadow: rgba(0, 0, 0, 0.24) 0px 3px 8px;"">
                                    Verify Email
                                    </a>
                                </div>
                            </div>
                    </section>
                </body>
                </html>"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpServer, _port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_userName, _password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
