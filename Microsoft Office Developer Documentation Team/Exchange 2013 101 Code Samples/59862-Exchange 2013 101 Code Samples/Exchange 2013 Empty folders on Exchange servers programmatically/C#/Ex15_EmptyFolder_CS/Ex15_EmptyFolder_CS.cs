using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_EmptyFolder_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // In the Inbox folder, attempt to permanently delete all contents in the folder with the display name "Custom Folder".
            EmptyFolder(service, "Custom Folder", DeleteMode.HardDelete, true, WellKnownFolderName.Inbox);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void EmptyFolder(ExchangeService service, string DisplayName, DeleteMode deleteMode, bool DeleteSubFolders, WellKnownFolderName ParentFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (DisplayName) within the specified folder (ParentFolder).
            FolderId folderId = Ex15_FindFolderIdByDisplayName_CS.FindFolderIdByDisplayName(service, DisplayName, ParentFolder);

            if (folderId != null)
            {
                // Bind to the folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Delete all contents in the folder.
                folder.Empty(deleteMode, DeleteSubFolders); 

                Console.WriteLine("Folder '" + DisplayName + "' has been emptied.");
            }
            else
            {
                Console.WriteLine("Folder '" + DisplayName + "' could not be found in the '" + ParentFolder + "' folder.");
            }
        }

    }
}
