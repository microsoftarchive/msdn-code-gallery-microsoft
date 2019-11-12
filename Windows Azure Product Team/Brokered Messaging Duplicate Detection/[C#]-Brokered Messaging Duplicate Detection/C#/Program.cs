//---------------------------------------------------------------------------------
// Microsoft (R)  Windows Azure AppFabric SDK
// Software Development Kit
// 
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Description;

namespace Microsoft.Samples.DuplicateDetection
{
    class Program
    {
        static string serviceBusConnectionString;

        static void Main()
        {
            string queueNameDupDection = "RemoveDuplicatesQueue";
            string queueNameNoDupDection = "DefaultQueue";

 
            GetUserCredentials();

            // Get ServiceBusNamespaceClient for management operations
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);
            
            // 1. Create a queue without duplicate detection enabled
            Console.WriteLine("\nCreating {0} ...", queueNameNoDupDection);
            if (namespaceManager.QueueExists(queueNameNoDupDection))
            {
                namespaceManager.DeleteQueue(queueNameNoDupDection);
            }
            namespaceManager.CreateQueue(queueNameNoDupDection);
            Console.WriteLine("Created {0}", queueNameNoDupDection);

            // Get messageFactory for runtime operation
            MessagingFactory messagingFactory = MessagingFactory.CreateFromConnectionString(serviceBusConnectionString);

            SendReceive(messagingFactory, queueNameNoDupDection);

            // 2. Create a queue with duplicate detection enabled
            Console.WriteLine("\nCreating {0} ...", queueNameDupDection);
            if (namespaceManager.QueueExists(queueNameDupDection))
            {
                namespaceManager.DeleteQueue(queueNameDupDection);
            }
            namespaceManager.CreateQueue(new QueueDescription(queueNameDupDection) { RequiresDuplicateDetection = true });
            Console.WriteLine("Created {0}", queueNameDupDection);
            SendReceive(messagingFactory, queueNameDupDection);

            Console.WriteLine("\nPress [Enter] to exit.");
            Console.ReadLine();

            // Cleanup:
            namespaceManager.DeleteQueue(queueNameDupDection);
            namespaceManager.DeleteQueue(queueNameNoDupDection);
        }


        static void SendReceive(MessagingFactory messagingFactory, string queueName)
        {
            QueueClient queueClient = messagingFactory.CreateQueueClient(queueName);

            // Send messages to queue
            Console.WriteLine("\tSending messages to {0} ...", queueName);
            BrokeredMessage message = new BrokeredMessage();
            message.MessageId = "ABC123";
            queueClient.Send(message);
            Console.WriteLine("\t=> Sent a message with messageId {0}", message.MessageId);

            BrokeredMessage message2 = new BrokeredMessage();
            message2.MessageId = "ABC123";
            queueClient.Send(message2);
            Console.WriteLine("\t=> Sent a duplicate message with messageId {0}", message.MessageId);

            // Receive messages from queue
            string receivedMessageId = "";

            Console.WriteLine("\n\tWaiting for messages from {0} ...", queueName);
            while (true)
            {
                BrokeredMessage receivedMessage = queueClient.Receive(TimeSpan.FromSeconds(10));

                if (receivedMessage == null)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\t<= Received a message with messageId {0}", receivedMessage.MessageId);
                    receivedMessage.Complete();
                    if (receivedMessageId.Equals(receivedMessage.MessageId, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\t\tRECEIVED a DUPLICATE MESSAGE");
                    }

                    receivedMessageId = receivedMessage.MessageId;
                }
            }

            Console.WriteLine("\tDone receiving messages from {0}", queueName);

            queueClient.Close();
        }

        static void GetUserCredentials()
        {
            Console.Write("Please provide a connection string to Service Bus (/? for help):\n ");
            serviceBusConnectionString = Console.ReadLine();

            if ((String.Compare(serviceBusConnectionString, "/?") == 0) || (serviceBusConnectionString.Length == 0))
            {
                Console.Write("To connect to the Service Bus cloud service, go to the Windows Azure portal and select 'View Connection String'.\n");
                Console.Write("To connect to the Service Bus for Windows Server, use the get-sbClientConfiguration PowerShell cmdlet.\n\n");
                Console.Write("A Service Bus connection string has the following format: \nEndpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=<keyName>;SharedAccessKey=<key>");

                serviceBusConnectionString = Console.ReadLine();
                Environment.Exit(0);
            }
        }
    }
}
