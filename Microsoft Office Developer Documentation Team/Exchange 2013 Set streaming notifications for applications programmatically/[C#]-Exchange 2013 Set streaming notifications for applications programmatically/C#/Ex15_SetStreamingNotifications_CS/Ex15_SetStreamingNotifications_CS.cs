using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.    
    class Notifications
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        private static AutoResetEvent Signal; 


        static void Main(string[] args)
        {
            SetStreamingNotifications(service);

            // Wait for the application to exit
            Signal = new AutoResetEvent(false);
            Signal.WaitOne();

        }
        static void SetStreamingNotifications(ExchangeService service)
        {
            // Subscribe to streaming notifications on the Inbox folder, and listen 
            // for "NewMail", "Created", and "Deleted" events. 
            StreamingSubscription streamingsubscription = service.SubscribeToStreamingNotifications(
                new FolderId[] { WellKnownFolderName.Inbox },
                EventType.NewMail,
                EventType.Created,
                EventType.Deleted);

            StreamingSubscriptionConnection connection = new StreamingSubscriptionConnection(service, 1);

            connection.AddSubscription(streamingsubscription);
            // Delegate event handlers. 
            connection.OnNotificationEvent +=
                new StreamingSubscriptionConnection.NotificationEventDelegate(OnEvent);
            connection.OnSubscriptionError +=
                new StreamingSubscriptionConnection.SubscriptionErrorDelegate(OnError);
            connection.OnDisconnect +=
                new StreamingSubscriptionConnection.SubscriptionErrorDelegate(OnDisconnect);
            connection.Open();

            Console.WriteLine("--------- StreamSubscription event -------");
        }

        static private void OnDisconnect(object sender, SubscriptionErrorEventArgs args)
        {
            // Cast the sender as a StreamingSubscriptionConnection object.           
            StreamingSubscriptionConnection connection = (StreamingSubscriptionConnection)sender;
            // Ask the user if they want to reconnect or close the subscription. 
            ConsoleKeyInfo cki;
            Console.WriteLine("The connection to the subscription is disconnected.");
            Console.WriteLine("Do you want to reconnect to the subscription? Y/N");
            while (true)
            {
                cki = Console.ReadKey(true);
                {
                    if (cki.Key == ConsoleKey.Y)
                    {
                        connection.Open();
                        Console.WriteLine("Connection open.");
                        Console.WriteLine("\r\n");
                        break;
                    }
                    else if (cki.Key == ConsoleKey.N)
                    {
                        Signal.Set();
                        bool isOpen = connection.IsOpen;
                        
                        if (isOpen == true)
                        {
                        // Close the connection
                        connection.Close();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

        }

        static void OnEvent(object sender, NotificationEventArgs args)
        {
            StreamingSubscription subscription = args.Subscription;

            // Loop through all item-related events. 
            foreach (NotificationEvent notification in args.Events)
            {

                switch (notification.EventType)
                {
                    case EventType.NewMail:
                        Console.WriteLine("\n-------------Mail created:-------------");
                        break;
                    case EventType.Created:
                        Console.WriteLine("\n-------------Item or folder created:-------------");
                        break;
                    case EventType.Deleted:
                        Console.WriteLine("\n-------------Item or folder deleted:-------------");
                        break;
                }
                // Display the notification identifier. 
                if (notification is ItemEvent)
                {
                    // The NotificationEvent for an email message is an ItemEvent. 
                    ItemEvent itemEvent = (ItemEvent)notification;
                    Console.WriteLine("\nItemId: " + itemEvent.ItemId.UniqueId);
                }
                else
                {
                    // The NotificationEvent for a folder is a FolderEvent. 
                    FolderEvent folderEvent = (FolderEvent)notification;
                    Console.WriteLine("\nFolderId: " + folderEvent.FolderId.UniqueId);
                }
            }
        }
        static void OnError(object sender, SubscriptionErrorEventArgs args)
        {
            // Handle error conditions. 
            Exception e = args.Exception;
            Console.WriteLine("\n-------------Error ---" + e.Message + "-------------");
        } 

    }
}
