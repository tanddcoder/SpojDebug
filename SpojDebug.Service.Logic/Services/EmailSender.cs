using SpojDebug.Service.Email;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using SpojDebug.Business.Cache;

namespace SpojDebug.Service.Logic
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly IAdminSettingCacheBusiness _adminSettingCacheBusiness;

        public EmailSender(IAdminSettingCacheBusiness adminSettingCacheBusiness)
        {
            _adminSettingCacheBusiness = adminSettingCacheBusiness;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            var emailInfo = await _adminSettingCacheBusiness.GetEmailInfo();
            client.Credentials = new System.Net.NetworkCredential(emailInfo.Email, emailInfo.Password);

            MailMessage mm = new MailMessage(emailInfo.Email, email, subject, message);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }
    }
}
