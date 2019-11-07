using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_SharePointDeleterExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Before you run this WCF Client, ensure that the 
            //WCF_ItemDeleterService project has been started

            //Create the WCF Client
            Console.WriteLine("Connecting to the WCF SharePoint Item Deleter Service");
            SharePointItemDeleterClient sharePointClient = new SharePointItemDeleterClient();
            //Get the list entries from the WCF service
            Console.WriteLine("Deleting the item...");
            Console.WriteLine();
            //Change this line to pass the title of an item that exists in your list
            bool sharepointResults = sharePointClient.DeleteItem("Test Item");
            if (sharepointResults)
            {
                Console.WriteLine("The item was successfully deleted");
            }
            else
            {
                Console.WriteLine("The operation did not complete properly. Have you edited the service code to work with your SharePoint farm and list name?");
            }
            Console.WriteLine("Press any key to close the client application");
            Console.ReadKey();
        }
    }
}
