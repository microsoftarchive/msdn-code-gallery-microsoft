using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_MoveFolder_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Move the "Custom Folder" folder from the Inbox folder to the Drafts folder.
            MoveFolder(service, "Custom Folder", WellKnownFolderName.Inbox, WellKnownFolderName.Drafts);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void MoveFolder(ExchangeService service, string DisplayName, WellKnownFolderName SourceFolder, WellKnownFolderName DestinationFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (DisplayName) within the specified folder (SourceFolder).
            FolderId folderId = Ex15_FindFolderIdByDisplayName_CS.FindFolderIdByDisplayName(service, DisplayName, SourceFolder);

            if (folderId != null)
            {
                // Bind to the folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Move the folder from its current location into the specified location (DestinationFolder).
                Folder newFolder = folder.Move(DestinationFolder);

                Console.WriteLine("Folder '" + DisplayName + "' has been moved to the '" + DestinationFolder.ToString() + "' folder.");
            }
            else
            {
                Console.WriteLine("Folder '" + DisplayName + "' could not be found in the '" + SourceFolder + "' folder.");
            }
        }
    }
}
