using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
           SendSMS(service);
           Console.WriteLine("Press or select Enter..."); 
           Console.Read();
        }

        /// <summary>
        /// Creates and sends a text message.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void SendSMS(ExchangeService service)
        {
           // Each carrier has a different SMS gateway format and address. Review your mobile carrier's documentation
           // for more information about their specific SMS gateway format. This sample shows the AT&T gateway.
           // Note that text message rates are charged by running this code.
           string attMailtoSMSGateway = "txt.att.net";
           string phoneNumber = "4255551212";

           // Form the SMS message.
           EmailMessage message = new EmailMessage(service);
           message.ItemClass = "IPM.Note.Mobile.SMS";
           message.ToRecipients.Add(phoneNumber + "@" + attMailtoSMSGateway);
           message.Subject = "Test subject";
           message.Body = "Test body";
           message.Send();
        }
    }
}
