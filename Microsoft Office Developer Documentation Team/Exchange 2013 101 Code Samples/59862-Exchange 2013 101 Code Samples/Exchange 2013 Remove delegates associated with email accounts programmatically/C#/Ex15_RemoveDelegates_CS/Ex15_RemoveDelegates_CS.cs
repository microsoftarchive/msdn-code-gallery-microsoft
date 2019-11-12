using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;

namespace Exchange101
{
    // This sample is for demonstration purposes only.
    // Before you run this sample, make sure that the code meets the
    // coding requirements of your organization.
    static class Ex15_RemoveDelegates_CS
    {
        static void Main(string[] args)
        {

            Console.Title = "EWS Test Console";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WindowHeight = Console.LargestWindowHeight * 9 / 10;
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(200, 3000);

            ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData());
            Mailbox primaryMailbox = new Mailbox(UserDataFromConsole.UserData.EmailAddress);

            // Get a list of delegates from the primary mailbox.
            ICollection<DelegateUser> delegates = Exchange101.Ex_15_GetDelegates_CS.GetDelegates(service, primaryMailbox);

            // Get the UserIds of the delegates.
            List<UserId> delegateIds = new List<UserId>();
            foreach (DelegateUser delegateUser in delegates)
            {
                delegateIds.Add(delegateUser.UserId);
            }

            // Remove the delegates from the primary mailbox.
            RemoveCalendarEditDelegate(service, primaryMailbox, delegateIds.ToArray());

            Console.ReadLine();

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }

        // Removes the specified delegates from a mailbox.
        // The userIds parameter can be an array of delegate user IDs to remove, a single delegate user ID,
        // or a list of user IDS separated by commas.
        static void RemoveCalendarEditDelegate(ExchangeService service, Mailbox primaryMailbox, params UserId[] userIds)
        {
            service.RemoveDelegates(primaryMailbox, userIds);
        }
    }
}
