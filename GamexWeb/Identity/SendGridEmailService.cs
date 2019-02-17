using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GamexWeb.Identity
{
    public class SendGridEmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var apiKey = WebConfigurationManager.AppSettings["SendGridApiKey"]; 
            var senderEmail = "nhanttse62849@fpt.edu.vn";
            var from = new EmailAddress(senderEmail, "Gamex Administrator");
            var to = new EmailAddress(message.Destination);
            var subject = message.Subject;
            var client = new SendGridClient(apiKey);

            var myMessage = new SendGridMessage();
            myMessage.AddTo(to);
            myMessage.From = from;
            myMessage.Subject = subject;
            myMessage.PlainTextContent = message.Body;
            myMessage.HtmlContent = message.Body;

            myMessage.SetClickTracking(false, false);
            myMessage.SetOpenTracking(false);
            myMessage.SetSubscriptionTracking(false);

            await client.SendEmailAsync(myMessage);
        }
    }
}