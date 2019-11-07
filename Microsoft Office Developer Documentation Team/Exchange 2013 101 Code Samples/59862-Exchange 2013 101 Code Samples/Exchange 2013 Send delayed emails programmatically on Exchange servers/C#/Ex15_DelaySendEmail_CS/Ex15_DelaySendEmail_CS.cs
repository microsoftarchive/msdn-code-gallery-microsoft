using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using Exchange101;
using System.Collections.ObjectModel;
using System.Threading;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DelaySendEmail_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            // Delays the time for when an email is sent. 
            DelaySendEmail(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        
        static void DelaySendEmail(ExchangeService service)
        {
            // Create a new email message.
            EmailMessage message = new EmailMessage(service);

            // Specify the email recipient and subject.
            message.ToRecipients.Add("user1@contoso.com");
            message.Subject = "Test subject";

            // Identify the extended property that can be used to specify when to send the email.
            ExtendedPropertyDefinition PR_DEFERRED_SEND_TIME = new ExtendedPropertyDefinition(16367, MapiPropertyType.SystemTime);

            // Set the time that will be used to specify when the email is sent.
            // In this example, the email will be sent 1 minute after the next line executes,
            // provided that the message.SendAndSaveCopy request is processed by the server within 1 minute.
            string sendTime = DateTime.Now.AddMinutes(1).ToUniversalTime().ToString();

            // Specify when to send the email by setting the value of the extended property.
            message.SetExtendedProperty(PR_DEFERRED_SEND_TIME, sendTime);

            // Specify the email body.
            StringBuilder str = new StringBuilder();
            str.AppendLine("Client submitted the message.SendAndSaveCopy request at: " + DateTime.Now.ToUniversalTime().ToString() + ";");
            str.AppendLine(" email message will be sent at: " + sendTime + ".");
            message.Body = str.ToString();

            Console.WriteLine("");
            Console.WriteLine("Client submitted the message.SendAndSaveCopy request at: " + DateTime.Now.ToUniversalTime().ToString() + ".");
            Console.WriteLine("Email message will be sent at: " + sendTime + ".");

            // Submit the request to send the email message.
            message.SendAndSaveCopy();
        }
    }
}
