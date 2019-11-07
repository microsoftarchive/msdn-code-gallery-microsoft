using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    class Ex15_ArchiveItems_CS
    {
        // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
        
        // Note that for this sample, the ExchangeVersion is hard-coded in UserData.cs to ExchangeVersion.Exchange2013.

        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            ArchiveItemsInFolder(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void ArchiveItemsInFolder(ExchangeService service)
        {
            // Get the item IDs of the individual items to archive.

                // Create a search filter.
                List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
                searchFilterCollection.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, "Contoso"));
                SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, searchFilterCollection);

                // create an item view with the properties to return.
                ItemView view = new ItemView(50);
                view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject);
                view.Traversal = ItemTraversal.Shallow;
                
                // Get item IDs for the items in your Inbox with "Contoso" in the subject.
                FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);

                List<ItemId> itemIds = new List<ItemId>();

                foreach (Item item in findResults)
                {
                    itemIds.Add(item.Id);
                }

            // Archive the items that match your search.

                ServiceResponseCollection<ArchiveItemResponse> archivedItems = service.ArchiveItems(itemIds, WellKnownFolderName.Inbox);

                // Note that archiving must be enabled for the target user or archiving will fail.
                if (archivedItems.OverallResult != ServiceResult.Success)
                {
                    // Display any errors.
                    foreach (ArchiveItemResponse response in archivedItems)
                    {
                        if (response.ErrorCode != ServiceError.NoError)
                        {
                            Console.WriteLine("Error message for item " + "{0}: " + "{1}", response.Item, response.ErrorMessage);
                        }
                    }
                }
                else
                    // Display the number of items archived.
                {
                    Console.WriteLine("{0}" + " items archived.", archivedItems.Count);
                }
        }
    }
}
