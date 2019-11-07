using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15_FindFolderIdByDisplayName_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Attempt to retrieve the unique identifier of the folder with display name "Custom Folder" (located in the Inbox folder).
            FolderId folderId = FindFolderIdByDisplayName(service, "Custom Folder", WellKnownFolderName.Inbox);

            if (folderId != null)
            {
                Console.WriteLine("The unique identifier of the 'Custom Folder' folder (in the Inbox folder) is: " + folderId.ToString());
            }
            else
            {
                Console.WriteLine("The 'Custom Folder' folder was not found in the Inbox folder.");
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        public static FolderId FindFolderIdByDisplayName(ExchangeService service, string DisplayName, WellKnownFolderName SearchFolder)
        {
            // Specify the root folder to be searched.
            Folder rootFolder = Folder.Bind(service, SearchFolder);

            // Loop through the child folders of the folder being searched.
            foreach (Folder folder in rootFolder.FindFolders(new FolderView(100)))
            {
                // If the display name of the current folder matches the specified display name, return the folder's unique identifier.
                if (folder.DisplayName==DisplayName)
                {
                    return folder.Id;
                }
            }

            // If no folders have a display name that matches the specified display name, return null.
            return null;
        }
    }
}
