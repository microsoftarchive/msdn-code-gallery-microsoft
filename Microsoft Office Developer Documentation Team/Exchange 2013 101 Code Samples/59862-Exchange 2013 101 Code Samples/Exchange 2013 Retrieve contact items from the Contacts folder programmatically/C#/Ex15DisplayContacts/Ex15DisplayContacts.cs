using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15DisplayContacts
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DisplayContacts(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }
        private static void DisplayContacts(ExchangeService service)
        {
            // Get the number of items in the Contacts folder. To keep the response smaller, request only the TotalCount property.
            ContactsFolder contactsfolder = ContactsFolder.Bind(service,
                                                                WellKnownFolderName.Contacts,
                                                                new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));

            // Set the number of items to the smaller of the number of items in the Contacts folder or 1000.
            int numItems = contactsfolder.TotalCount < 1000 ? contactsfolder.TotalCount : 1000;

            // Instantiate the item view with the number of items to retrieve from the Contacts folder.
            ItemView view = new ItemView(numItems);

            // To keep the request smaller, send only the display name.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ContactSchema.DisplayName);

            // Retrieve the items in the Contacts folder that have the properties you've selected.
            FindItemsResults<Item> contactItems = service.FindItems(WellKnownFolderName.Contacts, view);

            // Display the display name of all the contacts. (Note that there can be a large number of contacts in the Contacts folder.)
            foreach (Item item in contactItems)
            {
                if (item is Contact)
                {
                    Contact contact = item as Contact;
                    Console.WriteLine(contact.DisplayName);
                }
            }
        }
    }
}
