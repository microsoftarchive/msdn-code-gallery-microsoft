using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    public class Ex15_GetChildFoldersUnderRoot_CS
    {
        // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.

        // Note that for this sample, the ExchangeVersion is hard-coded in UserData.cs to ExchangeVersion.Exchange2013.
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetChildFolders(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void GetChildFolders(ExchangeService service)
        {
            // Get the message folder root.
            Folder rootfolder = Folder.Bind(service, WellKnownFolderName.MsgFolderRoot);

            // Set the properties you want to retrieve when you load the folder.
            PropertySet propsToLoad = new PropertySet(FolderSchema.DisplayName,
                                                      FolderSchema.ChildFolderCount,
                                                      FolderSchema.FolderClass,
                                                      // Note that you don't display the folder IDs because they're very large,
                                                      // but retrieve them because they can be useful in other methods you might call.
                                                      FolderSchema.Id);

            // Get the root folder with the selected properties.
            rootfolder.Load(propsToLoad);            
            
            // Load the number of subfolders unless there are more than 100.
            int numSubFoldersToView = rootfolder.ChildFolderCount <= 100 ? rootfolder.ChildFolderCount : 100;

            // Display the child folders under the root, the number of subfolders under each child, and the folder class of each child folder.
            Console.WriteLine("\n" + "Folder Name".PadRight(28) + "\t" + "subfolders".PadRight(12) + "Folder Class" + "\n");

            foreach (Folder childFolder in rootfolder.FindFolders(new FolderView(numSubFoldersToView)))
            {
                Console.WriteLine(childFolder.DisplayName.PadRight(28) + "\t" + childFolder.ChildFolderCount.ToString().PadRight(12) + childFolder.FolderClass);
            }
        }
    }
}
