using System.Threading.Tasks;

namespace SpojDebug.Service.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
