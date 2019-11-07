using System;
using System.Security;

using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample code, make sure
    // that the code meets the requirements of your organizaton.
    public static class Ex15_ConnectToService_CS
    {
        static void Main(string[] args)
        {
            Console.Title = "EWS Test Console";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WindowHeight = Console.LargestWindowHeight * 9 / 10;
            Console.WindowWidth = Console.LargestWindowWidth / 2;
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(200, 3000);

            // Get the user's email address, credentials, and an email address to impersonate (if any).
            Ex15_CTS_IUserData_CS userData = Ex15_CTS_UserDataFromConsole_CS.Get();

            ExchangeService exchangeService = null;

            // Create an ExchangeService object that connects to the EWS endpoint using the
            // credentials and impersontation information from the user.
            if (userData.UseDefaultCredentials)
            {
                if (string.IsNullOrEmpty(userData.ImpersonatedEmailAddress))
                {
                    Console.WriteLine(string.Format("Connecting to service for {0} using default credentials.", userData.EmailAddress));
                    exchangeService = Ex15_CTS_Service_CS.ConnectToService(userData.EmailAddress);
                }
                else
                {
                    Console.WriteLine(string.Format("Connecting to service as {0} using default credentials.", userData.ImpersonatedEmailAddress));
                    exchangeService = Ex15_CTS_Service_CS.ConnectToService(userData.EmailAddress, userData.ImpersonatedEmailAddress);
                }
            }
            else 
            {
                if (string.IsNullOrEmpty(userData.ImpersonatedEmailAddress))
                {
                    Console.WriteLine(string.Format("Connecting to service for {0} using entered credentials.", userData.EmailAddress));
                    exchangeService = Ex15_CTS_Service_CS.ConnectToService(userData.EmailAddress, userData.Credentials);
                }
                else
                {
                    Console.WriteLine(string.Format("Connecting to service as {0} using entered credentials.", userData.ImpersonatedEmailAddress));
                    exchangeService = Ex15_CTS_Service_CS.ConnectToService(userData.EmailAddress, userData.Credentials, userData.ImpersonatedEmailAddress);
                }
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.ReadLine();
        }
    }
}
