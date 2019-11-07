using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;
using System.IO;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_FindAssociated_CS
    {

        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
       
        static void Main(string[] args)
        {
            FindAssociated(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void FindAssociated(ExchangeService service)
        {
           // Create a view with a page size of 100.
           ItemView view = new ItemView(100);

           // Indicate that the base property will be the item identifier
           view.PropertySet = new PropertySet(BasePropertySet.FirstClassProperties);
           view.PropertySet.Add(ItemSchema.IsAssociated);


           // Set the traversal to associated. (Shallow is the default option; other options are Associated and SoftDeleted.)
           view.Traversal = ItemTraversal.Associated;

           // Send the request to search the Inbox.
           FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, view);
           
            // Output a list of the item classes for the associated items
            foreach (Item item in findResults)
           {
               Console.WriteLine(item.ItemClass);
           }
        }

    }
}
