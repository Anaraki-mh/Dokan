namespace Dokan.Services
{
    public interface IEmailService
    {
        void SendEmail(string subject, string body, string recipientEmail);
    }
}