using ActiveOfficeLife.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppConfigService _config;

        public EmailService(AppConfigService config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var smtp = new SmtpClient
            {
                Host = _config.AppConfigs.EmailSmtp.SmtpHost,
                Port = _config.AppConfigs.EmailSmtp.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config.AppConfigs.EmailSmtp.Username,
                    _config.AppConfigs.EmailSmtp.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config.AppConfigs.EmailSmtp.From, _config.AppConfigs.EmailSmtp.FromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await smtp.SendMailAsync(mail);
        }
    }

}
