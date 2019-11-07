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
    class Ex15_UpdateEmail_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateAndUpdateEmail(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void CreateAndUpdateEmail(ExchangeService service)
        {
            // Create a new email message.
            EmailMessage message = new EmailMessage(service);

            // Set properties on the email message.
            message.ToRecipients.Add("user1@contoso.com");
            message.Subject = "Project priorities";
            message.Body = "(1) Buy pizza, (2) Eat pizza.";

            // Save the email message to the Drafts folder. 
            message.Save(WellKnownFolderName.Drafts);

            Console.WriteLine("");
            Console.WriteLine("A draft email message with the subject '" + message.Subject + "' has been saved to the Drafts folder.");

            // Update properties on the email message (locally).
            message.Importance = Importance.High;
            message.Body = "(1) Start Environmental Impact Statement, (2) Identify preferred option.";

            // Apply the updates made locally to the email message on the server.
            message.Update(ConflictResolutionMode.AutoResolve);

            Console.WriteLine("");
            Console.WriteLine("Updates to the email message (Importance, Body) have been saved to the server.");
        }
    }
}
