using Microsoft.Exchange.WebServices.Data;
using System;

namespace Exchange101
{
    // This sample is for demonstration purposes only.
    // Before you run this sample, make sure that the code meets the
    // coding requirements of your organization.
    static class Ex15_AddDelegates_CS
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

            AddCalendarEditDelegate(service, primaryMailbox, "user1@contoso.com");

            Console.ReadLine();

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }

        // Adds a delegate to the primary email account that receives calendar requests
        // and has permission to edit the primary email account's calendar.
        static void AddCalendarEditDelegate(ExchangeService service, Mailbox primaryMailbox, string delegateEmailAddress)
        {
            DelegateUser calendarDelegate = new DelegateUser(delegateEmailAddress);
            calendarDelegate.ReceiveCopiesOfMeetingMessages = true;
            calendarDelegate.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;

            service.AddDelegates(primaryMailbox, MeetingRequestsDeliveryScope.DelegatesAndMe, calendarDelegate);
        }
    }
}
