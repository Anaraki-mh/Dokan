using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Dokan.Services
{
    public class EmailService : IEmailService
    {
        #region Fields and Properties

        private SmtpClient _smtpClient { get; }
        private string _hostName { get; }
        private string _username { get; }
        private string _password { get; }
        private string _senderEmail { get; }

        #endregion


        #region Constructor

        public EmailService()
        {
            _hostName = "smtp.gmail.com";
            _username = WebConfigurationManager.AppSettings["EmailUsername"];
            _password = WebConfigurationManager.AppSettings["EmailPassword"];
            _senderEmail = "";

            _smtpClient = new SmtpClient(_hostName)
            {
                Port = 587,
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true,
            };
        }

        #endregion


        #region Methods

        public void SendEmail(string subject, string body, string recipientEmail)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,

                };

                mailMessage.To.Add(recipientEmail);

                _smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}
