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
        private string _senderEmail { get; }

        #endregion


        #region Constructor

        public EmailService()
        {
            _senderEmail = WebConfigurationManager.AppSettings["EmailUsername"];
            var emailHost = WebConfigurationManager.AppSettings["EmailHost"];
            var username = WebConfigurationManager.AppSettings["EmailUsername"];
            var password = WebConfigurationManager.AppSettings["EmailPassword"];
            var hasSpecificPort = bool.Parse(WebConfigurationManager.AppSettings["EmailHostHasSpecificPort"]);
            var hostPort = int.Parse(WebConfigurationManager.AppSettings["EmailHostPort"]);

            _smtpClient = new SmtpClient(emailHost)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            if (hasSpecificPort)
            {
                _smtpClient.Port = hostPort;
            }
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
