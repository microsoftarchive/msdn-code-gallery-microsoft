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
    class Ex15_DeleteEmail_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // In the Inbox folder, delete items where the subject contains the word "advertisement".
            DeleteEmails(service, WellKnownFolderName.Inbox, "subject:advertisement");

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        
        
        static void DeleteEmails(ExchangeService service, WellKnownFolderName ParentFolder, String SearchCriteria)
        {
            // Find items in the specified folder (ParentFolder) that match the specified search criteria (SearchCriteria).
            // This example assumes that the search response returns exactly 3 results.
            FindItemsResults<Item> results = service.FindItems(ParentFolder, SearchCriteria, new ItemView(3));

            if (results.TotalCount != 3)
            {
                Console.WriteLine();
                Console.WriteLine("Found " + results.TotalCount.ToString() + " items in the " + ParentFolder + " folder that match the specified search criteria (" + SearchCriteria + ").");
                Console.WriteLine();
                Console.WriteLine("For this example to work as designed, update the search criteria so that exactly 3 items in the " + ParentFolder + " folder match the specified search criteria.");
                return;
            }
            else
            {
                Console.WriteLine("Found exactly 3 items in the " + ParentFolder + " folder that match the search criteria (" + SearchCriteria + ").");
                Console.WriteLine();
            }

            // In all versions of Exchange, there are 3 methods of deleting an item:  1) hard delete, 2) move to deleted items, and 3) soft delete.
            // The result of each method, however, differs depending on the (major) version of Exchange Server for the mailbox where the item exists.

            // Perform deletions against an Exchange Server 2007 mailbox. 
            if (service.ServerInfo.MajorVersion == 12)
            {
                Console.WriteLine("Major server version is Exchange Server 2007.");
                Console.WriteLine();

                // Using the search results, hard delete the first item.
                // In Exchange 2007, an item that is deleted by using DeleteMode.HardDelete is permanently deleted from the store. 
                results.Items[0].Delete(DeleteMode.HardDelete);

                // Using the search results, move the second item to the Deleted Items folder.
                results.Items[1].Delete(DeleteMode.MoveToDeletedItems);

                // Using the search results, soft delete the third item.
                // In Exchange 2007, an item that is deleted by using DeleteMode.SoftDelete continues to exist in the same folder,
                // but can only be found by performing a FindItem operation with a soft-delete traversal.
                // The item cannot be moved, copied, or restored by using Exchange Web Services.
                results.Items[2].Delete(DeleteMode.SoftDelete);

                Console.WriteLine("3 items that matched the search criteria (" + SearchCriteria + ") were deleted from the " + ParentFolder + " folder.");
            }

            // Perform deletions against an Exchange Server 2010 mailbox.
            if (service.ServerInfo.MajorVersion == 14)
            {
                Console.WriteLine("Major server version is Exchange Server 2010.");
                Console.WriteLine();

                // Using the search results, hard delete the first item.
                // In Exchange 2010, an item that is deleted by using DeleteMode.HardDelete is moved to the Recoverable Items\Purges folder.
                results.Items[0].Delete(DeleteMode.HardDelete);

                // Using the search results, move the second item to the Deleted Items folder.
                results.Items[1].Delete(DeleteMode.MoveToDeletedItems);

                // Using the search results, soft delete the third item.
                // In Exchange 2010, an item that is deleted by using DeleteMode.SoftDelete is moved to the Recoverable Items\Deletions folder.
                results.Items[2].Delete(DeleteMode.SoftDelete);

                Console.WriteLine("3 items that matched the search criteria (" + SearchCriteria + ") were deleted from the " + ParentFolder + " folder.");
            }
        }
    }
}
