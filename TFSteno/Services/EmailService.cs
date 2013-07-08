using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using TFSteno.Models;

namespace TFSteno.Services
{
    public static class EmailService
    {
        private static readonly string _SmtpServer;
        private static readonly int _SmtpPort;
        private static readonly string _SmtpUsername;
        private static readonly string _SmtpPassword;

        static EmailService()
        {
            _SmtpServer = ConfigurationManager.AppSettings["smtpServer"];
            _SmtpPort = Int32.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            _SmtpUsername = ConfigurationManager.AppSettings["smtpUsername"];
            _SmtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
        }

        public static void SendConfirmationEmail(string to, string confirmationCode)
        {
            string body = "Please click the following link to confirm your email address: https://tfsteno.azurewebsites.net/Signup/Confirm?confirmationCode=" + confirmationCode;
            SendEmail("admin@tfsteno.azurewebsites.net", null, to, "TF Steno Email Confirmation", body);
            
        }

        public static void SendFailureEmail(WorkItemEmail workItemEmail)
        {
            SendEmail(
                "admin@tfsteno.azurewebsites.net", null, workItemEmail.From,
                "[TFSteno] Failed to save work item",
                String.Format("Failed to save your data.\r\nWork Item Id: {0}; History Text: {1}", workItemEmail.WorkItemId, workItemEmail.HistoryText));
        }

        private static void SendEmail(string fromEmailAddress, string replyToEmailAddress, string toEmailAddress, string subject, string body)
        {
            var mailMessage = new MailMessage(fromEmailAddress, toEmailAddress, subject, body);
            if (!String.IsNullOrWhiteSpace(replyToEmailAddress))
                mailMessage.ReplyToList.Add(replyToEmailAddress);

            using (var smtpClient = new SmtpClient(_SmtpServer, _SmtpPort))
            {
                smtpClient.EnableSsl = (_SmtpPort == 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_SmtpUsername, _SmtpPassword);
                smtpClient.Send(mailMessage);
            }
        }

        
    }
}