using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_SharePointInserterExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Before you run this WCF Client, ensure that the 
            //WCF_ItemCreatorService project has been started

            //Create the WCF Client
            Console.WriteLine("Connecting to the WCF SharePoint Item Inserter Service");
            SharePointItemInserterClient sharePointClient = new SharePointItemInserterClient();
            //Get the list entries from the WCF service
            Console.WriteLine("Inserting the item...");
            Console.WriteLine();
            bool sharepointResults = sharePointClient.InsertItem("Test Item", "This item was created by a WCF service");
            if (sharepointResults)
            {
                Console.WriteLine("The item was successfully created and saved");
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
