using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Notifications
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            SetPullNotifications(service);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }
        static void SetPullNotifications(ExchangeService service)
        {
            // Initiate the subscription object, specifying the folder and events.
            PullSubscription subscription = service.SubscribeToPullNotifications(
                new FolderId[] { WellKnownFolderName.Inbox }, 5, null,
                EventType.NewMail, EventType.Created, EventType.Deleted);

            // Initiate the GetEvents method for the new subscription.
            GetEventsResults events = subscription.GetEvents();

            // Handle the results of the GetEvents method.
            foreach (ItemEvent itemEvent in events.ItemEvents)
            {
                switch (itemEvent.EventType)
                {
                    case EventType.NewMail:
                        EmailMessage message = EmailMessage.Bind(service, itemEvent.ItemId);
                        break;
                    case EventType.Created:
                        Item item = Item.Bind(service, itemEvent.ItemId);
                        break;
                    case EventType.Deleted:
                        Console.WriteLine("Item deleted: " + itemEvent.ItemId.UniqueId);
                        break;
                }
            }
        }
    }
}
