using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15FindContactByDisplayName
    {
        // Acquire an email address and password from the console. This will be used to instantiate a service object.
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Change the name "Brian Johnson" to the display name of the contact you're looking for.
            if (FindContactByDisplayName(service, "Brian Johnson") != null)
            {
                Console.WriteLine("\r\nUnique contact found.");
            }
            else
            {
                Console.WriteLine("\r\nUnique contact not found.");
            }
        
            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        public static Contact FindContactByDisplayName(ExchangeService service, string DisplayName)
        {
            // Get the number of items in the Contacts folder. To keep the response smaller, request only the TotalCount property.
            ContactsFolder contactsfolder = ContactsFolder.Bind(service,
                                                                WellKnownFolderName.Contacts,
                                                                new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));

            // Set the number of items to the smaller of the number of items in the contacts folder or 1000.
            int numItems = contactsfolder.TotalCount < 1000 ? contactsfolder.TotalCount : 1000;

            // Instantiate the item view with the number of items to retrieve from the Contacts folder.
            ItemView view = new ItemView(numItems);

            // To keep the request smaller, send only the display name.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ContactSchema.DisplayName);

            //Create a searchfilter.
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(ContactSchema.DisplayName, DisplayName);

            // Retrieve the items in the contacts folder with the properties you've selected.
            FindItemsResults<Item> contactItems = service.FindItems(WellKnownFolderName.Contacts, filter, view);
            if (contactItems.Count() == 1) //Only one contact was found
            {
                return (Contact)contactItems.Items[0];
            }
            else //No contacts or more than one contact with the same DisplayName were found.
            {
                return null;
            }
        }
    }
}
