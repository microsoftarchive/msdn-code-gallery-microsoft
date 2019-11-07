using System;
using Microsoft.Exchange.WebServices.Data;


namespace Exchange101
{
    class Ex15_SyncMailboxes_CS
    {
        // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            SyncMailboxes(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void SyncMailboxes(ExchangeService service)
        {
            // Retrieve a collection of changes from the server for the Inbox, including the first-class properties. When the synch state parameter is null,
            // changes for all subfolders under the specified folder will be retrieved.
            ChangeCollection<FolderChange> folderChangeCollection = service.SyncFolderHierarchy(new FolderId(WellKnownFolderName.Inbox), PropertySet.FirstClassProperties, null);

            // Display changes, if any. Note that instead of displaying the changes,
            // you can create, update, or delete folders based on the changes retrieved from the server.
            if (folderChangeCollection.Count != 0)
            {
                foreach (FolderChange fc in folderChangeCollection)
                {
                    Console.WriteLine("ChangeType: " + fc.ChangeType.ToString());
                    Console.WriteLine("DisplayName: " + fc.Folder.DisplayName);
                    Console.WriteLine("ChildFolderCount: " + fc.Folder.ChildFolderCount);
                    Console.WriteLine("===========");
                }
            }
        }
    }
}
