using System;
using System.Security;

using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample code, make sure
    // that the code meets the requirements of your organizaton.
    public static class Ex_15_GetDelegates_CS
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

            ICollection<DelegateUser> delegates = GetDelegates(service, primaryMailbox);
            if (delegates != null)
            {
                foreach (DelegateUser user in delegates)
                {
                    Console.WriteLine(user.ToString());
                }
            }
            else
            {
                Console.WriteLine(string.Format("No delegates for account {0}", UserDataFromConsole.UserData.EmailAddress));
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }


        public static ICollection<DelegateUser> GetDelegates(ExchangeService service, Mailbox primaryAccount)
        {
            DelegateInformation response = service.GetDelegates(primaryAccount, true);

            List<DelegateUser> delegateUsers = null;

            if (response.DelegateUserResponses.Count > 0)
            {
                delegateUsers = new List<DelegateUser>();

                foreach (DelegateUserResponse delegateResponse in response.DelegateUserResponses)
                {
                    delegateUsers.Add(delegateResponse.DelegateUser);
                }
            }

            return delegateUsers;
        }
    }
}
