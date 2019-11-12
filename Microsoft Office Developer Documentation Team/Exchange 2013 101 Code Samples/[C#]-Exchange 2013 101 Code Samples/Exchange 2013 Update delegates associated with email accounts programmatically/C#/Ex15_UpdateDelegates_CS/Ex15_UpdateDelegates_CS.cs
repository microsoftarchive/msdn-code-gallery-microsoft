using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;

namespace Exchange101
{
    // This sample is for demonstration purposes only.
    // Before you run this sample, make sure that the code meets the
    // coding requirements of your organization.
    static class Ex15_UpdateDelegates_CS
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

            ICollection<DelegateUser> delegates = Exchange101.Ex_15_GetDelegates_CS.GetDelegates(service, primaryMailbox);

            if (delegates != null)
            {
                foreach (DelegateUser delegateUser in delegates)
                {
                    if (delegateUser.Permissions.CalendarFolderPermissionLevel == DelegateFolderPermissionLevel.Editor)
                    {
                        delegateUser.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                    }
                }

                service.UpdateDelegates(primaryMailbox, MeetingRequestsDeliveryScope.DelegatesAndMe, delegates);
            }
            else
            {
                Console.Write(string.Format("No delegates found for address {0}.", UserDataFromConsole.UserData.EmailAddress));
            }


            Console.ReadLine();

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }

    }
}
