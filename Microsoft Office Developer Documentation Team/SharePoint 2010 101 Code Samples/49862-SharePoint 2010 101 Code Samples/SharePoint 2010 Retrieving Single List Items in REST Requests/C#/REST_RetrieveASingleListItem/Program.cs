using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Needed System.Net for the CredentialsCache class
using System.Net;
//This includes the Team Site Service Reference
using REST_RetrieveASingleListItem.TeamSiteServiceReference;

namespace REST_RetrieveASingleListItem
{
    class Program
    {
        static void Main(string[] args)
        {
            //This example returns an item called "Test Item" from the Announcements list
            //For your own lists, first add a Service Reference to the SharePoint site you want to query
            //Then change the item object below.

            //Formulate the URL to the List Data RESTful service 
            string Url = "http://intranet.contoso.com/_vti_bin/listdata.svc";
            //Create a dataContext object for the service
            TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri(Url));
            //Authenticate as the currently logged on user
            dataContext.Credentials = CredentialCache.DefaultCredentials;
            //Query for a single item by using a lambda expression to select items with a specific title
            AnnouncementsItem item = dataContext.Announcements.Where(i => i.Title == "Test Item").FirstOrDefault();
            if (item == null)
            {
                //There is no announcement with that title
                Console.WriteLine("An announcement with the title 'Test Item' was not found.");
            }
            else
            {
                //Return the item
                Console.WriteLine("Title: " + item.Title);
                Console.WriteLine("Body: " + item.Body);
            }
            //This line prevents the console disappearing before you can read the result
            //Alternatively, remove this line a run the project without debugging (CTRL-F5)
            Console.ReadKey();
        }
    }
}
