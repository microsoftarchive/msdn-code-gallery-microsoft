using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//This is needed for the credentials classes
using System.Net;
//This is the main WCF namespace
using System.ServiceModel;
//This namespace is used for self-hosting the service
using System.ServiceModel.Description;
//This is the service reference to the SharePoint RESTful list service
using WCF_ItemCreatorService.TeamSiteServiceReference;

namespace WCF_ItemCreatorService
{
    class Program
    {
        //Define the service contract interface
        [ServiceContract(Namespace = "http://WCF_SharePointCallingService")]
        public interface ISharePointItemInserter
        {
            //One method that inserts an item and returns true or false
            [OperationContract]
            bool InsertItem(string Title, string Body);
        }

        public class SharePointItemInserterService : ISharePointItemInserter
        {
            public bool InsertItem(string Title, string Body)
            {
                //This example inserts a new item into the Announcements list
                //For your own lists, first add a Service Reference to the 
                //SharePoint site you want to query, then change the item object below.

                //Tell the user what's going on
                Console.WriteLine("Received a InsertItem call. About to query SharePoint...");
                //Formulate the URL to the List Data RESTful service.
                //You must correct this path to point to your own SharePoint farm
                string Url = "http://intranet.contoso.com/_vti_bin/listdata.svc";
                //Create a dataContext object for the service
                TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri(Url));
                //Authenticate as administrator. 
                NetworkCredential myCredential = new NetworkCredential("Administrator", "pass@word1");
                dataContext.Credentials = myCredential;
                //Create a new item
                AnnouncementsItem newAnnouncement = new AnnouncementsItem();
                newAnnouncement.Title = Title;
                newAnnouncement.Body = Body;
                try
                {
                    //Add it to the Announcements list
                    dataContext.AddToAnnouncements(newAnnouncement);
                    dataContext.SaveChanges();
                    Console.WriteLine("Added this new item: {0}", Title);
                    //Return the dictionary
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: {0}", e.Message);
                    return false;
                }
            }
        }

        static void Main(string[] args)
        {
            //This example WCF service is self-hosted in
            //the command console. This procedure runs the
            //service until the user presses a key

            //This is the address for the WCF service
            Uri baseAddress = new Uri("http://localhost:8088/WCF_SharePointItemInserterService/Service");
            ServiceHost selfHost = new ServiceHost(typeof(SharePointItemInserterService), baseAddress);
            try
            {
                //Create an endpoint
                selfHost.AddServiceEndpoint(typeof(ISharePointItemInserter), new WSHttpBinding(), "SharePointItemInserterService");
                //Enable the service to exchange its metadata
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);
                //Open the service and tell the user
                selfHost.Open();
                Console.WriteLine("The SharePoint Item Inserter Service is ready");
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
