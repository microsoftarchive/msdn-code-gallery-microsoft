using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15GetRooms
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetRooms(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }


        private static void GetRooms(ExchangeService service)
        {
            // Return all the room lists in the organization.
            EmailAddressCollection roomLists = service.GetRoomLists();

            // Retrieve the room list that matches your criteria.
            // Replace "ConfRoomsIn31@contoso.com" with the room list you are looking for.
            EmailAddress roomAddress = new EmailAddress("ConfRoomsIn31@contoso.com");
            foreach (EmailAddress address in roomLists)
            {
                if (address == roomAddress)
                {
                    Console.WriteLine("Found {0} in room list", roomAddress);
                }
                else
                {
                    Console.WriteLine("No matching room list found.");
                    return;
                }
            }

            // Expand the selected collection to get a list of rooms.
            System.Collections.ObjectModel.Collection<EmailAddress> roomAddresses = service.GetRooms(roomAddress);

            // Display the individual rooms.
            foreach (EmailAddress address in roomAddresses)
            {
                Console.WriteLine("Email Address: {0}", address.Address);
            }
        }
    }
}
