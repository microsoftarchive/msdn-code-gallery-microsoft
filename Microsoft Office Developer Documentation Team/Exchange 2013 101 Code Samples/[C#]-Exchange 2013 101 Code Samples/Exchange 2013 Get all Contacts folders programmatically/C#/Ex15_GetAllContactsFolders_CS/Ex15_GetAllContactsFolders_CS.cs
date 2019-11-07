using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_GetAllContactsFolders_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetAllContactsFolders(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Performs a deep-traversal search of the default Contacts folder and returns a 
        /// collection of all contact folders.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void GetAllContactsFolders(ExchangeService service)
        {
            // Collection will contain all contact folders. 
            Collection<Folder> folders = new Collection<Folder>();
            
            // Get the root Contacts folder and load all properties. This results in a GetFolder operation call to EWS.
            ContactsFolder rootContactFolder = ContactsFolder.Bind(service, WellKnownFolderName.Contacts);
            folders.Add(rootContactFolder);
            Console.WriteLine("Added the default Contacts folder to the collection of contact folders.");

            // Find all child folders of the root Contacts folder.
            int initialFolderSearchOffset = 0;
            const int folderSearchPageSize = 100;
            bool AreMoreFolders = true;
            FolderView folderView = new FolderView(folderSearchPageSize, initialFolderSearchOffset);
            folderView.Traversal = FolderTraversal.Deep;
            folderView.PropertySet = new PropertySet(BasePropertySet.IdOnly);

            while (AreMoreFolders)
            {
                try
                {
                    // Find all the child folders of the default Contacts folder. This results in a FindFolder operation call to EWS.
                    FindFoldersResults childrenOfContactsFolderResults = rootContactFolder.FindFolders(folderView);
                    if (folderView.Offset == 0)
                    {
                        Console.WriteLine("Found {0} child folders of the default Contacts folder.", childrenOfContactsFolderResults.TotalCount);
                    }

                    foreach (Folder f in childrenOfContactsFolderResults.Folders)
                    {
                        ContactsFolder contactFolder = (ContactsFolder)f;
                        // Loads all the properties for the folder. This results in a GetFolder operation call to EWS.
                        contactFolder.Load();
                        Console.WriteLine("Loaded a folder named {0} and added it to the collection of contact folders.", contactFolder.DisplayName);
                        // Add the folder to the collection of contact folders.
                        folders.Add(contactFolder);
                    }

                    // Turn off paged searches if there are no more folders to return.
                    if (childrenOfContactsFolderResults.MoreAvailable == false)
                    {
                        AreMoreFolders = false;
                    }
                    else // Increment the paging offset.
                    {
                        folderView.Offset = folderView.Offset + folderSearchPageSize;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
        }
    }
}
