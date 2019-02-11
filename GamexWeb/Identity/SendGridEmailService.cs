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
            //            // Create the email object first, then add the properties.

            //            
            //            // this defines email and name of the sender
            //            myMessage.From = new MailAddress("no-reply@tech.trailmax.info", "My Awesome Admin");
            //            
            //            // set where we are sending the email
            //            myMessage.AddTo(message.Destination);
            //            
            //            myMessage.Subject = message.Subject;
            //            
            //            // make sure all your messages are formatted as HTML
            //            myMessage.HtmlContent = message.Body;
            //            
            //            // Create credentials, specifying your SendGrid username and password.
            //            var credentials = new NetworkCredential("YourUsername", "YourPassword");
            //            
            //            // Create an Web transport for sending email.
            //            var transportWeb = new Web(credentials);
            //            
            //            // Send the email.
            //            await transportWeb.DeliverAsync(myMessage);


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

            await client.SendEmailAsync(myMessage);
        }
    }
}