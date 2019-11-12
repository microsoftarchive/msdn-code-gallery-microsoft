//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------------
using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Microsoft.Samples.AdvancedFiltersSample
{
    class Program
    {
        #region Fields
        static string ServiceBusConnectionString;

        const string TopicName = "MyTopic";
        const string SubsNameAllMessages = "AllOrders";
        const string SubsNameColorBlueSize10Orders = "ColorBlueSize10Orders";
        const string SubsNameHighPriorityOrders = "HighPriorityOrders";
        #endregion

        static void Main(string[] args)
        {
            // ***************************************************************************************
            // This sample demonstrates how to use advanced filters with ServiceBus topics and subscriptions.
            // The sample creates a topic and 3 subscriptions with different filter definitions.
            // Each receiver will receive matching messages depending on the filter associated with a subscription.
            // ***************************************************************************************

            // Get ServiceBus namespace and credentials from the user.
            Program.GetNamespaceAndCredentials();

            // Create messaging factory and ServiceBus namespace client.
            MessagingFactory messagingFactory = Program.CreateMessagingFactory();
            NamespaceManager namespaceManager = Program.CreateNamespaceManager();

            // Delete the topic from last run.
            Program.DeleteTopicsAndSubscriptions(namespaceManager);
            
            // Create topic and subscriptions that'll be using through the sample.
            Program.CreateTopicsAndSubscriptions(namespaceManager);

            // Send sample messages.
            Program.SendMessagesToTopic(messagingFactory);

            // Receive messages from subscriptions.
            Program.ReceiveAllMessagesFromSubscripions(messagingFactory);

            messagingFactory.Close();

            Console.WriteLine("Press [Enter] to quit...");
            Console.ReadLine();
        }

        #region Helper Methods

        static void SendMessagesToTopic(MessagingFactory messagingFactory)
        {
            // Create client for the topic.
            TopicClient topicClient = messagingFactory.CreateTopicClient(Program.TopicName);

            // Create a message sender from the topic client.

            Console.WriteLine("\nSending orders to topic.");

            // Now we can start sending orders.
            SendOrder(topicClient, new Order());
            SendOrder(topicClient, new Order() { Color = "blue", Quantity = 5, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "red", Quantity = 10, Priority = "high" });
            SendOrder(topicClient, new Order() { Color = "yellow", Quantity = 5, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "blue", Quantity = 10, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "blue", Quantity = 5, Priority = "high" });
            SendOrder(topicClient, new Order() { Color = "blue", Quantity = 10, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "red", Quantity = 5, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "red", Quantity = 10, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "red", Quantity = 5, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "yellow", Quantity = 10, Priority = "high" });
            SendOrder(topicClient, new Order() { Color = "yellow", Quantity = 5, Priority = "low" });
            SendOrder(topicClient, new Order() { Color = "yellow", Quantity = 10, Priority = "low" });

            Console.WriteLine("All messages sent.");
        }

        static void SendOrder(TopicClient topicClient, Order order)
        {
            using (BrokeredMessage message = new BrokeredMessage())
            {
                message.CorrelationId = order.Priority;
                message.Properties.Add("color", order.Color);
                message.Properties.Add("quantity", order.Quantity);

                topicClient.Send(message);
            }

            Console.WriteLine("Sent order with Color={0}, Quantity={1}, Priority={2}", order.Color, order.Quantity, order.Priority);
        }

        static void ReceiveAllMessagesFromSubscripions(MessagingFactory messagingFactory)
        {
            // Receive message from 3 subscriptions.
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Program.SubsNameAllMessages);
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Program.SubsNameColorBlueSize10Orders);
            Program.ReceiveAllMessageFromSubscription(messagingFactory, Program.SubsNameHighPriorityOrders);
        }

        static void ReceiveAllMessageFromSubscription(MessagingFactory messagingFactory, string subsName)
        {
            int receivedMessages = 0;

            // Create subscription client.
            SubscriptionClient subsClient = 
                messagingFactory.CreateSubscriptionClient(Program.TopicName, subsName, ReceiveMode.ReceiveAndDelete);

            // Create a receiver from the subscription client and receive all messages.
            Console.WriteLine("\nReceiving messages from subscription {0}.", subsName);

            while (true)
            {
                BrokeredMessage receivedMessage;

                receivedMessage = subsClient.Receive(TimeSpan.FromSeconds(10));

                if (receivedMessage != null)
                {
                    receivedMessage.Dispose();
                    receivedMessages++;
                }
                else
                {
                    // No more messages to receive.
                    break;
                }
            }

            Console.WriteLine("Received {0} messages from subscription {1}.", receivedMessages, subsClient.Name);
        }

        static void CreateTopicsAndSubscriptions(NamespaceManager namespaceManager)
        {
            Console.WriteLine("\nCreating a topic and 3 subscriptions.");

            // Create a topic and 3 subscriptions.
            TopicDescription topicDescription = namespaceManager.CreateTopic(Program.TopicName);
            Console.WriteLine("Topic created.");

            // Create a subscription for all messages sent to topic.
            namespaceManager.CreateSubscription(topicDescription.Path, SubsNameAllMessages, new TrueFilter());
            Console.WriteLine("Subscription {0} added with filter definition set to TrueFilter.", Program.SubsNameAllMessages);

            // Create a subscription that'll receive all orders which have color "blue" and quantity 10.
            namespaceManager.CreateSubscription(topicDescription.Path, SubsNameColorBlueSize10Orders, new SqlFilter("color = 'blue' AND quantity = 10"));
            Console.WriteLine("Subscription {0} added with filter definition \"color = 'blue' AND quantity = 10\".", Program.SubsNameColorBlueSize10Orders);

            // Create a subscription that'll receive all high priority orders.
            namespaceManager.CreateSubscription(topicDescription.Path, SubsNameHighPriorityOrders, new CorrelationFilter("high"));
            Console.WriteLine("Subscription {0} added with correlation filter definition \"high\".", Program.SubsNameHighPriorityOrders);

            Console.WriteLine("Create completed.");
        }

        static void DeleteTopicsAndSubscriptions(NamespaceManager namespaceManager)
        {
            Console.WriteLine("\nDeleting topic and subscriptions from previous run if any.");

            try
            {
                namespaceManager.DeleteTopic(Program.TopicName);
            }
            catch (MessagingEntityNotFoundException)
            {
                Console.WriteLine("No topic found to delete.");
            }

            Console.WriteLine("Delete completed.");
        }

        static NamespaceManager CreateNamespaceManager()
        {
            return NamespaceManager.CreateFromConnectionString(ServiceBusConnectionString);
        }

        static MessagingFactory CreateMessagingFactory()
        {
             return MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
        }

        static void GetNamespaceAndCredentials()
        {
            Console.Write("Please provide a connection string to Service Bus (/? for help):\n ");
            Program.ServiceBusConnectionString = Console.ReadLine();

            if ((String.Compare(Program.ServiceBusConnectionString, "/?") == 0) || (Program.ServiceBusConnectionString.Length == 0))
            {
                Console.Write("To connect to the Service Bus cloud service, go to the Windows Azure portal and select 'View Connection String'.\n");
                Console.Write("To connect to the Service Bus for Windows Server, use the get-sbClientConfiguration PowerShell cmdlet.\n\n");
                Console.Write("A Service Bus connection string has the following format: \nEndpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyName>;SharedAccessKey=<key>\n");

                Program.ServiceBusConnectionString = Console.ReadLine();
                Environment.Exit(0);
            }
        }

        #endregion
    }
}
