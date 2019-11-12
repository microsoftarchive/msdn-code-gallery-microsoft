using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is needed for the credentials classes
using System.Net;
//This is the main WCF namespace
using System.ServiceModel;
//This namespace is used for self-hosting the service
using System.ServiceModel.Description;
//This is the Service Reference for the SharePoint RESTful List Service
using WCF_SharePointCallingService.TeamSiteServiceReference;

namespace WCF_SharePointCallingService
{
    class Program
    {
        //Define the service contract interface
        [ServiceContract(Namespace = "http://WCF_SharePointCallingService")]
        public interface ISharePointListReader
        {
            //One method that returns a dictionary of item Titles and Bodies.
            [OperationContract]
            Dictionary<string, string> GetList(string Name);
        }

        //This class implements the service contract defined in ISharePointListReader
        public class SharePointListReaderService : ISharePointListReader
        {
            //
            public Dictionary<string, string> GetList(string Name)
            {
                //Run a query against the SharePoint list
                Console.WriteLine("Received a GetList call. About to query SharePoint...");
                Dictionary<string, string> results = queryListServer(Name);
                //Let the user know what happened
                Console.WriteLine();
                Console.WriteLine("Query complete. Returning results to client.");
                return results;
            }

            private Dictionary<string, string> queryListServer(string listName)
            {
                //This example returns all items from the Announcements list
                //For your own lists, first add a Service Reference to the 
                //SharePoint site you want to query, then change the item object below.

                Dictionary<string, string> listEntries = new Dictionary<string, string>();
                //Formulate the URL to the List Data RESTful service 
                string Url = "http://intranet.contoso.com/_vti_bin/listdata.svc";
                //Create a dataContext object for the service
                TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri(Url));
                //Authenticate as administrator. 
                NetworkCredential myCredential = new NetworkCredential("Administrator", "pass@word1");
                dataContext.Credentials = myCredential;
                //As this is running in the console, we can display the results
                Console.WriteLine("Items in the Announcements list:");
                Console.WriteLine();
                //Loop through the announcements
                foreach (AnnouncementsItem item in dataContext.Announcements)
                {
                    //Display the item's properties
                    Console.WriteLine("Title: " + item.Title);
                    listEntries.Add(item.Title, item.Body);
                }
                //Return the dictionary
                return listEntries;
            }
        }

        static void Main(string[] args)
        {
            //This example WCF service is self-hosted in
            //the command console. This procedure runs the
            //service until the user presses a key

            //This is the address for the WCF service
            Uri baseAddress = new Uri("http://localhost:8088/WCF_SharePointCallingService/Service");
            ServiceHost selfHost = new ServiceHost(typeof(SharePointListReaderService), baseAddress);
            try
            {
                //Create an endpoint
                selfHost.AddServiceEndpoint(typeof(ISharePointListReader), new WSHttpBinding(), "SharePointListReaderService");
                //Enable the service to exchange its metadata
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);
                //Open the service and tell the user
                selfHost.Open();
                Console.WriteLine("The SharePoint List Reader Service is ready");
                Console.WriteLine("Press any key to close the service");
                //Wait for the user to press a key
                Console.ReadKey();
                //Close the service
                selfHost.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("A communication exception occurred: {0}", e.Message);
                selfHost.Abort();
            }

        }
    }
}
