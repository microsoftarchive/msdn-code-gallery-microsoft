using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    class Ex15_SetOOF_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Setting OOF
            SetOOF(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press any key...");
            Console.Read();
        }

        static void SetOOF(ExchangeService service)
        {
            OofSettings userOOF = new OofSettings();

            // Select the OOF status to be a set time period.
            userOOF.State = OofState.Scheduled;

            // Select the time period to be OOF
            userOOF.Duration = new TimeWindow(DateTime.Now.AddDays(4), DateTime.Now.AddDays(5));

            // Select the external audience that will receive OOF messages.
            userOOF.ExternalAudience = OofExternalAudience.All;

            // Select the OOF reply for your internal audience.
            userOOF.InternalReply = new OofReply("I'm currently out of office. Please contact my manager for critical issues. Thanks!");

            // Select the OOF reply for your external audience.
            userOOF.ExternalReply = new OofReply("I am currently out of the office but will reply to emails when I return. Thanks!");

            // Set the selected values. This method will result in a call to the Exchange Server.
            service.SetUserOofSettings(UserDataFromConsole.GetUserData().EmailAddress, userOOF);

            // Retrieve the user status and print to the console
            Ex15_GetOOF_CS.GetOOF(service);
        }

    }
}
