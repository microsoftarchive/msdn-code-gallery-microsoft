using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_ExpandDistributionLists_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            ExpandDistributionLists(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void ExpandDistributionLists(ExchangeService service)
        {
            // Retrieve the expanded group from the server.
            // Note that an invalid group will throw an exception which should be handled.
            ExpandGroupResults groupMembers = null;

            try
            {
             // Note: Replace "Group1@contoso.com" with the distribution list you wish to expand.
               groupMembers = service.ExpandGroup("Group1@contoso.com");
            }
            catch (ServiceResponseException SREx)
            {
                if (SREx.ErrorCode == ServiceError.ErrorNameResolutionNoResults)
                {
                    Console.WriteLine("The email address does not contain a valid group: {0}", SREx.ErrorCode);
                    return;
                }
                else
                {
                    throw (SREx);
                }
            }
            finally
            {
                if (groupMembers != null)
                    // Display the group members to the console.
                    foreach (EmailAddress address in groupMembers.Members)
                    {
                        Console.WriteLine("Email Address: {0}", address.Address);
                    }
            }
        }
    }
}
