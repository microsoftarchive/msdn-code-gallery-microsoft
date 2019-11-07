using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15_GetOOF_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            GetOOF(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        public static void GetOOF(ExchangeService service)
        {
            // Return the Out Of Office object that contains OOF state for the user whose credendials were supplied at the console. 
            // This method will result in a call to the Exchange Server.
            OofSettings userOOFSettings = service.GetUserOofSettings(UserDataFromConsole.GetUserData().EmailAddress);

            // Get the (read-only) audience of email message senders outside a client's organization who will receive automatic Out Of Office replies ("All", "Known", or "None").
            OofExternalAudience allowedExternalAudience = userOOFSettings.AllowExternalOof;

            // Get the duration for a scheduled Out Of Office reply.
            TimeWindow OOFDuration = userOOFSettings.Duration;

            // Get the ExternalAudience of email message senders outside a client's organization who will receive automatic Out OF Office replies (All/Known/None).
            OofExternalAudience externalAudience = userOOFSettings.ExternalAudience;

            // Get the reply to be sent to email message senders outside a client's organization.
            OofReply externalReply = userOOFSettings.ExternalReply;

            // Get the reply to be sent to email message senders inside a client's organization.
            OofReply internalReply = userOOFSettings.InternalReply;

            // Get the (Disabled/Enabled/Scheduled) state of the Out Of Office automatic reply feature.
            OofState userOofState = userOOFSettings.State;

            // Print user status information to the console
            Console.WriteLine("Allowed External Audience: {0}", allowedExternalAudience);
            Console.WriteLine("Out of Office duration: {0}", OOFDuration);
            Console.WriteLine("External Audience: {0}", externalAudience);
            Console.WriteLine("External Reply: {0}", externalReply);
            Console.WriteLine("Internal Reply: {0}", internalReply);
            Console.WriteLine("User OOF state: {0}", userOofState);
        }
    }
}
