using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15UpdateContact
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Change the name "Brian Johnson" to the display name of the contact you want to update.
            UpdateContact(service, "Brian Johnson");
        
            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        private static void UpdateContact(ExchangeService service, string DisplayName)
        {
            // Look for the contact to update. If the contact doesn't exist, create it.
            Contact contact = Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName);

            if (contact == null)
            {
                Ex15CreateContact.CreateContact(service);
                contact = Contact.Bind(service, Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName).Id);
            } 
            
            // Change the company name to an empty string.
            contact.CompanyName = "";
            // Commit the changes on the server.
            contact.Update(ConflictResolutionMode.AlwaysOverwrite);
            Console.WriteLine("Contact updated.");
        }
    }
}
