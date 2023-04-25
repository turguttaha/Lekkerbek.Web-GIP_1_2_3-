using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Quartz;

namespace Lekkerbek.Web.Services
{
    public class EmailService 
    {
       
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




