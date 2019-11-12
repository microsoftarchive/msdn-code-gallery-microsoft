using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is needed for the CredentialCache class
using System.Net;
//This is the service reference to SharePoint
using REST_RetrieveAllListItems.TeamSiteServiceReference;

namespace REST_RetrieveAllListItems
{
    class Program
    {
        static void Main(string[] args)
        {
            //This example returns all items from the Announcements list
            //For your own lists, first add a Service Reference to the SharePoint site you want to query
            //Then change the item object below.

            //Formulate the URL to the List Data RESTful service 
            string Url = "http://intranet.contoso.com/_vti_bin/listdata.svc";
            //Create a dataContext object for the service
            TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri(Url));
            //Authenticate as the currently logged on user
            dataContext.Credentials = CredentialCache.DefaultCredentials;
            //Tell the user what will be displayed
            Console.WriteLine("Items in the Announcements list:");
            Console.WriteLine();
            //Loop through the announcements
            foreach (AnnouncementsItem item in dataContext.Announcements)
            {
                //Display the item's properties
                Console.WriteLine("Title: " + item.Title);
                Console.WriteLine("ID: " + item.Id);
                Console.WriteLine("Body: " + item.Body);
                Console.WriteLine();
            }
            //This line prevents the console disappearing before you can read the result
            //Alternatively, remove this line a run the project without debugging (CTRL-F5)
            Console.ReadKey();
        }
    }
}
