using System.Net.Mail;
using System.Net;

namespace Lekkerbek.Web.Services
{
    public class EmailService
    {
        
        public void SendMail(string EmailTo, string subject, string bodyMail) 
        { 
            string fromMail = "gipteam2.lekkerbek@gmail.com";
            string fromPassword = "pagwjgwdlutmgpfj";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(EmailTo));
            message.Body = bodyMail;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
        
        
    }
}
