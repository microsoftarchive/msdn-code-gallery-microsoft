using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class ResolveNames
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            ResolveName(service, "Smith");

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void ResolveName(ExchangeService service, string NameToResolve)
        {
            // Identify the mailbox folders to search for potential name resolution matches.
            List<FolderId> folders = new List<FolderId>() { new FolderId(WellKnownFolderName.Contacts) };

            // Search for all contact entries in the default mailbox Contacts folder and in Active Directory Domain Services. This results in a call to EWS.
            NameResolutionCollection coll = service.ResolveName(NameToResolve, folders, ResolveNameSearchLocation.ContactsThenDirectory, false);

            foreach (NameResolution nameRes in coll)
            {
                Console.WriteLine("Contact name: " + nameRes.Mailbox.Name);
                Console.WriteLine("Contact e-mail address: " + nameRes.Mailbox.Address);
                Console.WriteLine("Mailbox type: " + nameRes.Mailbox.MailboxType);
            }
        }
    }
}
