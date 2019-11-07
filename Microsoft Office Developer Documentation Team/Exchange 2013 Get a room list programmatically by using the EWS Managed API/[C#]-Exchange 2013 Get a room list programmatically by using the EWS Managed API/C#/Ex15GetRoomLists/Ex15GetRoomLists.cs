using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class EX15GetRoomLists
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetRoomLists(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        private static void GetRoomLists(ExchangeService service)
        {
            // Return all the room lists in the organization.
            EmailAddressCollection roomLists = service.GetRoomLists();

            // Display the room lists.
            foreach (EmailAddress address in roomLists)
            {
                Console.WriteLine("Email Address: {0} Mailbox Type: {1}", address.Address, address.MailboxType);
            }
        }
    }
}
