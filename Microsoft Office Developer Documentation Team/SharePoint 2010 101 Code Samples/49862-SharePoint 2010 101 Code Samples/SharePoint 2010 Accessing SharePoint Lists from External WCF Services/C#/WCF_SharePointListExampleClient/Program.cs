using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_SharePointListExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Before you run this WCF Client, ensure that the 
            //WCF_SharePointCallingService project has been started

            //Create the WCF Client
            Console.WriteLine("Connecting to the WCF SharePoint List Reader Service");
            SharePointListReaderClient sharePointClient = new SharePointListReaderClient();
            //Alter this to the name of the list you want to access
            string listName = "Announcements";
            //Get the list entries from the WCF service
            Console.WriteLine("Getting list items...");
            Console.WriteLine();
            Dictionary<string, string> sharepointResults = sharePointClient.GetList(listName);
            foreach (KeyValuePair<string, string> pair in sharepointResults)
            {
                Console.WriteLine("Item:");
                Console.WriteLine("Title: {0}", pair.Key);
                Console.WriteLine("Body: {0}", pair.Value);
                Console.WriteLine();
            }
            //This line keeps the client open until the user presses a key
            Console.WriteLine("Press any key to close the client");
            Console.ReadKey();
        }
    }
}
