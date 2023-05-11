using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Quartz;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Azure.Core;

namespace Lekkerbek.Web.Services
{
    public class EmailService : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Execute(email, subject, htmlMessage);
            
        }

        public async Task Execute( string email, string subject, string htmlMessage)
        {
            string fromMail = "gipteam2.lekkerbek@gmail.com";
            string fromPassword = "pagwjgwdlutmgpfj";

            MailMessage mailMessage = new MailMessage(fromMail, email, subject, htmlMessage);
            mailMessage.IsBodyHtml = true;

            //MailMessage message = new MailMessage();
            //message.Body = htmlMessage;
            //message.From = new MailAddress(fromMail);
            //message.Subject = subject;
            //message.To.Add(new MailAddress(email));



            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(mailMessage);
        }

        public void SendMail(string EmailTo, string subject, string bodyMail, MailMessage a)
        {
        string fromMail = "gipteam2.lekkerbek@gmail.com";
        string fromPassword = "pagwjgwdlutmgpfj";

            MailMessage message = new MailMessage();
        a.From = new MailAddress(fromMail);
        a.Subject = subject;
        a.To.Add(new MailAddress(EmailTo));



        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };

        smtpClient.Send(a);
        }
    }
}




