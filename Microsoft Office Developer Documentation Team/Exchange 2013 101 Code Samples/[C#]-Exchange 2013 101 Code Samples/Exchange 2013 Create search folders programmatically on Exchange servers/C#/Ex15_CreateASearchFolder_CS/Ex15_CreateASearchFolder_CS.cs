using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class SearchFastSearch
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateASearchFolder(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Creates a search folder that finds items in the Inbox that contain the word "extended" in the subject line.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CreateASearchFolder(ExchangeService service)
        {
            // Create a new search folder.
            SearchFolder searchFolder = new SearchFolder(service);

            // Use the following search filter to get all mail in the Inbox with the word "extended" in the subject line.
            SearchFilter.ContainsSubstring searchCriteria = new SearchFilter.ContainsSubstring(ItemSchema.Subject, "extended");

            searchFolder.SearchParameters.RootFolderIds.Add(WellKnownFolderName.Inbox);
            searchFolder.SearchParameters.Traversal = SearchFolderTraversal.Shallow;
            searchFolder.SearchParameters.SearchFilter = searchCriteria;
            searchFolder.DisplayName = "Extended";

            try
            {
                // This call results in a CreateFolder operation call to EWS. The search folder should be added
                // to the WellKnownFolderName.SearchFolders folder so that it is visible to clients like Outlook. 
                searchFolder.Save(WellKnownFolderName.SearchFolders);
                Console.WriteLine("Added: {0}", searchFolder.DisplayName);
            }
            catch (ServiceResponseException e)
            {
                if (e.Response.ErrorCode == ServiceError.ErrorFolderExists)
                {
                    Console.WriteLine("Rename your search folder or delete the existing search folder of the same name.");
                }
                else
                    Console.WriteLine("Error - " + e.Message);
            }
        }
    }
}
