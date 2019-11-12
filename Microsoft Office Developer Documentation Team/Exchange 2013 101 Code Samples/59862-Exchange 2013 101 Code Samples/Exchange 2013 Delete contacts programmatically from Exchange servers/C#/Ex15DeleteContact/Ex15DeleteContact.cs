using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this code, make sure that meets the coding requirements of your organization.
    public class Ex15DeleteContact
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Change the name "Brian Johnson" to the display name of the contact you want to delete.
            // Note: Because the delete mode passed in is HardDelete, the contact will be permanently deleted.
            DeleteContact(service, "Brian Johnson", DeleteMode.HardDelete);
       
            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        private static void DeleteContact(ExchangeService service, string DisplayName, DeleteMode deleteMode)
        {
            // Look for the contact to delete. If it doesn't exist, create it.
            Contact contact = Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName);

            if (contact == null)
            {
                Ex15CreateContact.CreateContact(service);
                contact = Contact.Bind(service, Ex15FindContactByDisplayName.FindContactByDisplayName(service, DisplayName).Id);
            }

            // Delete the contact.
            contact.Delete(deleteMode);
            Console.WriteLine("Contact deleted.");
        }
    }
}
