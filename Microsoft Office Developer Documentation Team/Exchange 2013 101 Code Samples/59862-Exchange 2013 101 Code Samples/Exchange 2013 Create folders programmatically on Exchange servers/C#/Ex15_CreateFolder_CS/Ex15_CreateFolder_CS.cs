using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateFolder_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateFolder(service, "Custom Folder", WellKnownFolderName.Inbox);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void CreateFolder(ExchangeService service, string DisplayName, WellKnownFolderName DestinationFolder)
        {
            // Instantiate the Folder object.
            Folder folder = new Folder(service);

            // Specify the name of the new folder.
            folder.DisplayName = DisplayName;

            // Create the new folder in the specified destination folder.
            folder.Save(DestinationFolder);

            Console.WriteLine("Folder created:" + folder.DisplayName);
        }
    }
}
