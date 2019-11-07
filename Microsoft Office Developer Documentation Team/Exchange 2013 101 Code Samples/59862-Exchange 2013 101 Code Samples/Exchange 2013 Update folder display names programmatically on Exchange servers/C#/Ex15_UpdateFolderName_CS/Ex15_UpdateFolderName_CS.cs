using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UpdateFolderName_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // In the Inbox folder, change the name of the folder "Custom Folder" to "Custom Folder 2".
            UpdateFolderName(service, "Custom Folder", "Custom Folder 2", WellKnownFolderName.Inbox);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void UpdateFolderName(ExchangeService service, string CurrentDisplayName, string NewDisplayName, WellKnownFolderName ParentFolder)
        {
            // Attempt to retrieve the unique identifier of the folder with the specified display name (CurrentDisplayName) within the specified folder (ParentFolder).
            FolderId folderId = Ex15_FindFolderIdByDisplayName_CS.FindFolderIdByDisplayName(service, CurrentDisplayName, ParentFolder);

            if (folderId != null)
            {
                // Bind to the folder by using its unique identifier.
                Folder folder = Folder.Bind(service, folderId);

                // Change the display name of the folder.
                folder.DisplayName = NewDisplayName;

                // Save the change.
                folder.Update();

                Console.WriteLine("Folder name changed from '" + CurrentDisplayName + "' to '" + NewDisplayName + "'.");
            }
            else
            {
                Console.WriteLine("Folder '" + CurrentDisplayName + "' could not be found in the '" + ParentFolder + "' folder.");
            }
        }
    }
}
