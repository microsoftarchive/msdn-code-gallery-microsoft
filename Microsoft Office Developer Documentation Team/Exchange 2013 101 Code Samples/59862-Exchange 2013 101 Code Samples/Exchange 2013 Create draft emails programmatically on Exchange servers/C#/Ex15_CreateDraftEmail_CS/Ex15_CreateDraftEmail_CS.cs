using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;
using System.Collections.ObjectModel;
using System.Threading;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15_CreateDraftEmail_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Create an email message and save it in the Drafts folder.
            CreateDraftEmail(service, "user1@contoso.com", "Project priorities", "(1) Buy pizza, (2) Eat pizza");

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        public static void CreateDraftEmail(ExchangeService service, String To, String Subject, String Body)
        {
            // Create a new email message.
            EmailMessage message = new EmailMessage(service);

            // Set properties on the email message.
            message.ToRecipients.Add(To);
            message.Subject = Subject;
            message.Body = Body;

            // Save the email message to the Drafts folder (where it can be retrieved, updated, and sent at a later time). 
            message.Save(WellKnownFolderName.Drafts);

            Console.WriteLine("A draft email message with the subject '" + message.Subject + "' has been saved to the Drafts folder.");
        }
    }
}
