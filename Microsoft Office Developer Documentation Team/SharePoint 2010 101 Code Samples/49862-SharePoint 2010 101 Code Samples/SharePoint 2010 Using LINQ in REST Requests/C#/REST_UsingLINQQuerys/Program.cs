using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is needed for the CredentialCache class
using System.Net;
//This is the service reference to SharePoint
using REST_UsingLINQQuerys.TeamSiteServiceReference;

namespace REST_UsingLINQQuerys
{
    class Program
    {
        static void Main(string[] args)
        {
            //This example returns items from the Announcements list
            //For your own lists, first add a Service Reference to the SharePoint site you want to query
            //Then change the item object below.

            //Formulate the URL to the List Data RESTful service 
            string Url = "http://intranet.contoso.com/_vti_bin/listdata.svc";
            //Create a dataContext object for the service
            TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri(Url));
            //Authenticate as the currently logged on user
            dataContext.Credentials = CredentialCache.DefaultCredentials;
            //Select all items beginning with 'A' by using a Linq query
            var results = from items in dataContext.Announcements
                          where items.Title.StartsWith("A")
                          orderby items.Title
                          select new { items.Title, items.Id, items.Body };
            //Tell the user what will be displayed
            Console.WriteLine("Items in the Announcements list that begin with an A, ordered by Title:");
            Console.WriteLine();
            //Loop through the results
            foreach (var announcement in results)
            {
                //Display some properties of the item
                Console.WriteLine("Title: " + announcement.Title);
                Console.WriteLine("ID: " + announcement.Id);
                Console.WriteLine("Body: " + announcement.Body);
                Console.WriteLine();
            }
            //This line prevents the console disappearing before you can read the result
            //Alternatively, remove this line a run the project without debugging (CTRL-F5)
            Console.ReadKey();
        }
    }
}
