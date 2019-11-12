using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_SetFolderLevelPermissions_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Create a delegate user with folder permissions.
            UserId delegateUser = new UserId("delegateUser@contoso.com");
            FolderPermission permission = new FolderPermission(delegateUser, FolderPermissionLevel.Editor);

            // Create a new folder and add a delegate user with permission to edit.
            CreateFolder(service, "Custom Folder", WellKnownFolderName.Inbox, permission);

             Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void CreateFolder(ExchangeService service, string DisplayName, WellKnownFolderName DestinationFolder, FolderPermission Permissions)
        {
            // Instantiate the Folder object.
            Folder folder = new Folder(service);

            // Specify the name of the new folder.
            folder.DisplayName = DisplayName;

            // Add delegate permissions.
            folder.Permissions.Add(Permissions);

            // Create the new folder in the specified destination folder.
            folder.Save(DestinationFolder);

            Console.WriteLine("Folder created:" + folder.DisplayName);
        }
    }
}
