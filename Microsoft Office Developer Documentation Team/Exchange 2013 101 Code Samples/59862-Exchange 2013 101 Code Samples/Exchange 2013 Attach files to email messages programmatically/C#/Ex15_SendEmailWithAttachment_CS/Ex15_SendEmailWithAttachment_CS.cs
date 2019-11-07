using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Attachments
    {
        static IUserData UserData = UserDataFromConsole.GetUserData();
        static ExchangeService service = Service.ConnectToService(UserData, new TraceListener());

        static void Main(string[] args)
        {
            SendEmailWithAttachment(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void SendEmailWithAttachment(ExchangeService service)
        {
            //Create the email message.
            EmailMessage message = new EmailMessage(service);
            message.Subject = "Message with Attachments";
            message.Body = "This message contains one item attachment.";
            message.ToRecipients.Add(UserData.EmailAddress);

            //Create another item and use it as an attchment.
            ItemAttachment<EmailMessage> itemAttachment1 = message.Attachments.AddItemAttachment<EmailMessage>();
            itemAttachment1.Name = "Attached Message Item";
            itemAttachment1.Item.Subject = "Message Item Subject";
            itemAttachment1.Item.Body = "Message Item Body";
            itemAttachment1.Item.ToRecipients.Add(UserData.EmailAddress);

            message.SendAndSaveCopy();
            Console.WriteLine("Message sent with an attachment.");
        }
    }
}
