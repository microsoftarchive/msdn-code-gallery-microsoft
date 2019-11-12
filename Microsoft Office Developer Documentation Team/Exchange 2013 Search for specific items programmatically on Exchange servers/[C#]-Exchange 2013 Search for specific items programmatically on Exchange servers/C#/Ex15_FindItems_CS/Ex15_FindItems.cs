using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    class Ex15_FindItems
    {
        // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.

        // Note that for this sample, the ExchangeVersion is hard-coded in UserData.cs to ExchangeVersion.Exchange2013.
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            FindItems(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();

        }

        private static void FindItems(ExchangeService service)
        {
            // Create a search collection that contains your search conditions.
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, "Contoso"));
            searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Body, "Marketing"));

            // Create the search filter with a logical operator and your search parameters.
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.Or, searchFilterCollection.ToArray());

            // Limit the view to 50 items.
            ItemView view = new ItemView(50);

            // Limit the property set to the property ID for the base property set, and the subject and item class for the additional properties to retrieve.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.ItemClass);

            // Setting the traversal to shallow will return all non-soft-deleted items in the specified folder.
            view.Traversal = ItemTraversal.Shallow;

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);

            // Display each item.
            Console.WriteLine("\n" + "Item type".PadRight(50) + "\t" + "Subject");
            foreach (Item myItem in findResults.Items)
            {
                Console.WriteLine(myItem.GetType().ToString().PadRight(50) + "\t" + myItem.Subject.ToString());
            }            
        }
    }
}
