using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
           AddEmailSignatureToEmail(service);
           Console.WriteLine("Press or select Enter..."); 
           Console.Read();
        }

        /// <summary>
        /// Creates an email message and inserts an email signature into the email message body. This sample assumes that
        /// you use an HTML body for your email message.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void AddEmailSignatureToEmail(ExchangeService service)
        {
           // Create the email message text body.
           string htmlBodyTxt = @"<html><head></head><body><p>This is the email message body before a signature is added.</p>
                                </body></html>";

           // Identify the signature insertion point at the end of the HTML body.
           int signatureInsertPnt = htmlBodyTxt.IndexOf("</body>");

           // Create the email signature.
           string signature = "<p>Dallas Durkin<br/>Senior Planner<br/>Adventure Works Cycles</p>" +
                              "<p>4567 Main St.<br/>La Habra Heights, CA 90631</p><p>(323) 555-0100</p>";

           // Insert the signature into the HTML body.
           string newBody = htmlBodyTxt.Insert(signatureInsertPnt, signature);

           // Create the email message and set properties on it.
           EmailMessage email = new EmailMessage(service);
           email.ToRecipients.Add("user1@contoso.com");
           email.Subject = "Email with signature";
           email.Body = newBody;

           // Send the email message and save a copy in the default Sent Items folder. This results in a CreateItem call 
           // to EWS.
           email.SendAndSaveCopy();
        }
    }
}
